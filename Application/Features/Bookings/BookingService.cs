using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Enums;
using NewRentalCarManagerAPI.Infrastructure.Services;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Bookings;

// ══════════════════════════════════════════════
//  Booking
// ══════════════════════════════════════════════
public interface IBookingService
{
    Task<DataResult> GetAllAsync(BookingListInput input);
    Task<DataResult> GetByIdAsync(Guid id);
    Task<DataResult> GetByRenterAsync(Guid renterId, BookingListInput input);
    Task<DataResult> GetByCarAsync(Guid carId, BookingListInput input);
    Task<DataResult> CreateAsync(Guid renterId, CreateBookingDto dto);
    Task<DataResult> QuickCreateAsync(Guid renterId, QuickCreateBookingDto dto);
    Task<DataResult> UpdateAsync(Guid id, UpdateBookingDto dto);
    Task<DataResult> DeleteAsync(Guid id);
    Task<DataResult> PayBookingAsync(Guid bookingId, Guid payerId);
    Task<DataResult> SendEmailAsync(Guid bookingId);
}

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IBookingEmailQueue _emailQueue;
    private readonly ILogger<BookingService> _logger;
    public BookingService(IUnitOfWork uow, IMapper mapper, IBookingEmailQueue emailQueue, ILogger<BookingService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _emailQueue = emailQueue;
        _logger = logger;
    }

    private IQueryable<Booking> BaseQuery() => _uow.Bookings.Query()
        .Include(b => b.Renter).Include(b => b.Car).Include(b => b.Transactions);

    public async Task<DataResult> GetAllAsync(BookingListInput input)
    {
        try
        {
            var query = _uow.Bookings.Query().OrderByDescending(b => b.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount)
                .ProjectTo<BookingDto>(_mapper.ConfigurationProvider).ToListAsync();
            return DataResult.ResultSuccess(items, "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get bookings");
            throw;
        }
    }

    public async Task<DataResult> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await GetEntityByIdAsync(id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Booking not found!");
            return DataResult.ResultSuccess(_mapper.Map<BookingDto>(entity), "Get success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get booking {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> GetByRenterAsync(Guid renterId, BookingListInput input)
    {
        try
        {
            var query = _uow.Bookings.Query().Where(b => b.RenterId == renterId).OrderByDescending(b => b.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount)
                .ProjectTo<BookingDto>(_mapper.ConfigurationProvider).ToListAsync();
            return DataResult.ResultSuccess(items, "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get bookings by renter {RenterId}", renterId);
            throw;
        }
    }

    public async Task<DataResult> GetByCarAsync(Guid carId, BookingListInput input)
    {
        try
        {
            var query = _uow.Bookings.Query().Where(b => b.CarId == carId).OrderByDescending(b => b.CreatedAt);
            var totalCount = await query.CountAsync();
            var items = await query.Skip(input.SkipCount).Take(input.MaxResultCount)
                .ProjectTo<BookingDto>(_mapper.ConfigurationProvider).ToListAsync();
            return DataResult.ResultSuccess(items, "Get success!", totalCount);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get bookings by car {CarId}", carId);
            throw;
        }
    }

    public async Task<DataResult> CreateAsync(Guid renterId, CreateBookingDto dto)
    {
        try
        {
            // Acquire per-car advisory lock — held until transaction ends, works across distributed nodes
            var carLockId = BitConverter.ToInt64(dto.CarId.ToByteArray(), 0);
            await _uow.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock({carLockId})");

            var pricing = await _uow.CarPricings.GetByIdAsync(dto.PricingId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Pricing not found!");

            var hasOverlap = await _uow.CarAvailabilityBlocks.Query()
                .AnyAsync(b => b.CarId == dto.CarId && b.BlockedFrom < dto.RentalEnd && b.BlockedTo > dto.RentalStart);
            if (hasOverlap)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict, "Car is not available for the selected dates");

            var totalHours = (dto.RentalEnd - dto.RentalStart).TotalHours;
            var basePriceVnd = pricing.DurationHours.HasValue && pricing.DurationHours.Value > 0
                ? (int)Math.Ceiling(totalHours / pricing.DurationHours.Value) * pricing.PriceVnd
                : pricing.PriceVnd;

            var discountVnd = 0;
            if (dto.PromotionId.HasValue)
            {
                var promo = await _uow.Promotions.GetByIdAsync(dto.PromotionId.Value)
                    ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Promotion not found!");

                if (!promo.MinBookingVnd.HasValue || basePriceVnd >= promo.MinBookingVnd.Value)
                {
                    // Atomic increment with guard — eliminates UsedCount race condition
                    var promoId = dto.PromotionId.Value;
                    var rows = await _uow.ExecuteSqlAsync(
                        $"""
                        UPDATE booking.promotions
                           SET used_count = used_count + 1
                         WHERE id = {promoId}
                           AND is_active = true
                           AND now() BETWEEN valid_from AND valid_to
                           AND (usage_limit IS NULL OR used_count < usage_limit)
                        """);

                    if (rows == 0)
                        throw new UserFriendlyException((int)HttpStatusCode.Conflict, "Promotion is no longer valid or usage limit has been reached.");

                    discountVnd = promo.MaxDiscountVnd.HasValue
                        ? Math.Min(promo.DiscountValue, promo.MaxDiscountVnd.Value)
                        : promo.DiscountValue;
                }
            }

            var vatVnd = (int)((basePriceVnd - discountVnd) * 0.1);
            var totalPriceVnd = basePriceVnd - discountVnd + vatVnd;

            var entity = new Booking
            {
                RenterId = renterId, CarId = dto.CarId, PricingId = dto.PricingId,
                PromotionId = dto.PromotionId, PickupLocationId = dto.PickupLocationId,
                ReturnLocationId = dto.ReturnLocationId,
                RentalStart = dto.RentalStart, RentalEnd = dto.RentalEnd,
                BasePriceVnd = basePriceVnd, DiscountVnd = discountVnd,
                VatVnd = vatVnd, TotalPriceVnd = totalPriceVnd,
                DepositVnd = dto.DepositVnd, Note = dto.Note,
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            };
            await _uow.Bookings.AddAsync(entity);

            var block = new CarAvailabilityBlock
            {
                CarId = dto.CarId, BlockedFrom = dto.RentalStart,
                BlockedTo = dto.RentalEnd, Source = "booking"
            };
            await _uow.CarAvailabilityBlocks.AddAsync(block);
            block.BookingId = entity.Id;

            var created = await GetEntityByIdAsync(entity.Id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.InternalServerError, "Create booking failed.");
            return DataResult.ResultSuccess(_mapper.Map<BookingDto>(created), "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            if (e is not UserFriendlyException)
                _logger.LogError(e, "Failed to create booking");
            throw;
        }
    }

    public async Task<DataResult> QuickCreateAsync(Guid renterId, QuickCreateBookingDto dto)
    {
        try
        {
            var carLockId = BitConverter.ToInt64(dto.CarId.ToByteArray(), 0);
            await _uow.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock({carLockId})");

            var car = await _uow.Cars.Query()
                .Include(c => c.CarPricings)
                .FirstOrDefaultAsync(c => c.Id == dto.CarId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Car not found!");

            var rentalStart = DateTime.Parse(dto.StartDate).ToUniversalTime();
            var rentalEnd = DateTime.Parse(dto.EndDate).ToUniversalTime();

            var hasOverlap = await _uow.CarAvailabilityBlocks.Query()
                .AnyAsync(b => b.CarId == dto.CarId && b.BlockedFrom < rentalEnd && b.BlockedTo > rentalStart);
            if (hasOverlap)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict, "Car is not available for the selected dates");

            // Auto-pick active pricing (or first available)
            var pricing = car.CarPricings.FirstOrDefault(p => p.IsActive)
                ?? car.CarPricings.FirstOrDefault();

            int basePriceVnd;
            Guid pricingId;
            if (pricing is not null)
            {
                pricingId = pricing.Id;
                var totalHours = (rentalEnd - rentalStart).TotalHours;
                basePriceVnd = pricing.DurationHours.HasValue && pricing.DurationHours.Value > 0
                    ? (int)Math.Ceiling(totalHours / pricing.DurationHours.Value) * pricing.PriceVnd
                    : pricing.PriceVnd;
            }
            else
            {
                var defaultPricing = new CarPricing
                {
                    CarId = car.Id, DurationHours = 24, PriceVnd = 1000000, IsActive = true
                };
                await _uow.CarPricings.AddAsync(defaultPricing);
                pricingId = defaultPricing.Id;
                var totalDays = Math.Ceiling((rentalEnd - rentalStart).TotalDays);
                basePriceVnd = (int)totalDays * 1000000;
            }

            var vatVnd = (int)(basePriceVnd * 0.1);
            var totalPriceVnd = basePriceVnd + vatVnd;

            var entity = new Booking
            {
                RenterId = renterId, CarId = dto.CarId, PricingId = pricingId,
                PickupLocationId = car.LocationId, ReturnLocationId = car.LocationId,
                RentalStart = rentalStart, RentalEnd = rentalEnd,
                BasePriceVnd = basePriceVnd, DiscountVnd = 0,
                VatVnd = vatVnd, TotalPriceVnd = totalPriceVnd,
                DepositVnd = 0, Note = dto.Note,
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            };
            await _uow.Bookings.AddAsync(entity);

            var block = new CarAvailabilityBlock
            {
                CarId = dto.CarId, BlockedFrom = rentalStart,
                BlockedTo = rentalEnd, Source = "booking"
            };
            await _uow.CarAvailabilityBlocks.AddAsync(block);
            block.BookingId = entity.Id;

            var dtoResult = new BookingDto
            {
                Id = entity.Id, RenterId = renterId, RenterName = "",
                CarId = dto.CarId, CarLicensePlate = car.LicensePlate,
                PricingId = pricingId, PickupLocationId = car.LocationId,
                ReturnLocationId = car.LocationId,
                RentalStart = rentalStart, RentalEnd = rentalEnd,
                BasePriceVnd = basePriceVnd, DiscountVnd = 0,
                VatVnd = vatVnd, TotalPriceVnd = totalPriceVnd,
                DepositVnd = 0, Note = dto.Note,
                CreatedAt = entity.CreatedAt, UpdatedAt = entity.UpdatedAt,
                Status = entity.Status.ToString()
            };
            return DataResult.ResultSuccess(dtoResult, "Insert success!", statusCode: 201);
        }
        catch (Exception e)
        {
            if (e is not UserFriendlyException)
                _logger.LogError(e, "Failed to quick create booking");
            throw;
        }
    }

    public async Task<DataResult> UpdateAsync(Guid id, UpdateBookingDto dto)
    {
        try
        {
            var entity = await GetEntityByIdAsync(id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Booking not found!");
            if (dto.Status is not null && Enum.TryParse<BookingStatus>(dto.Status, true, out var newStatus))
            {
                entity.Status = newStatus;
            }
            if (dto.ActualReturnAt.HasValue) entity.ActualReturnAt = dto.ActualReturnAt;
            if (dto.CancelReason is not null) entity.CancelReason = dto.CancelReason;
            if (dto.Note is not null) entity.Note = dto.Note;
            entity.UpdatedAt = DateTime.UtcNow;
            return DataResult.ResultSuccess(_mapper.Map<BookingDto>(entity), "Update success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update booking {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _uow.Bookings.GetByIdAsync(id)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Booking not found!");
            _uow.Bookings.Remove(entity);
            return DataResult.ResultSuccess(true, "Delete success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete booking {Id}", id);
            throw;
        }
    }

    public async Task<DataResult> PayBookingAsync(Guid bookingId, Guid payerId)
    {
        try
        {
            var booking = await GetEntityByIdAsync(bookingId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Booking not found!");

            var alreadyPaid = booking.Transactions
                .Any(t => t.Status == Enums.PaymentStatus.Success && t.Direction == Enums.PaymentDirection.Charge);
            if (alreadyPaid)
            {
                var dto = _mapper.Map<BookingDto>(booking);
                dto.PaymentStatus = "Paid";
                dto.EmailStatus = "Sent";
                return DataResult.ResultSuccess(dto, "Get success!");
            }

            var paidAt = DateTime.UtcNow;
            var tx = new Transaction
            {
                BookingId = bookingId,
                PayerId = payerId,
                AmountVnd = booking.TotalPriceVnd,
                Direction = Enums.PaymentDirection.Charge,
                Method = Enums.PaymentMethod.BankTransfer,
                Status = Enums.PaymentStatus.Success,
                PaidAt = paidAt,
                CreatedAt = paidAt,
                Note = "Thanh toán trực tiếp"
            };
            await _uow.Transactions.AddAsync(tx);

            if (booking.Status == BookingStatus.Pending)
            {
                booking.Status = BookingStatus.Confirmed;
            }
            booking.UpdatedAt = paidAt;

            var emailStatus = "Queued";
            var renterEmail = booking.Renter?.Email;
            var renterName = booking.Renter?.FullName ?? "Khách hàng";
            var carName = booking.Car != null ? $"{booking.Car.LicensePlate}" : $"#{booking.CarId}";

            if (!string.IsNullOrEmpty(renterEmail))
            {
                await _emailQueue.EnqueueAsync(new BookingEmailJob(
                    BookingId: booking.Id,
                    RenterId: booking.RenterId,
                    RenterEmail: renterEmail,
                    RenterName: renterName,
                    CarName: carName,
                    RentalStart: booking.RentalStart,
                    RentalEnd: booking.RentalEnd,
                    TotalPriceVnd: booking.TotalPriceVnd,
                    PaidAt: paidAt));
            }
            else
            {
                emailStatus = "Skipped";
            }

            var resultDto = _mapper.Map<BookingDto>(booking);
            resultDto.PaymentStatus = "Paid";
            resultDto.EmailStatus = emailStatus;
            return DataResult.ResultSuccess(resultDto, "Pay success!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to pay booking {Id}", bookingId);
            throw;
        }
    }

    public async Task<DataResult> SendEmailAsync(Guid bookingId)
    {
        try
        {
            var booking = await GetEntityByIdAsync(bookingId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Booking not found!");

            var renterEmail = booking.Renter?.Email;
            if (string.IsNullOrEmpty(renterEmail))
            {
                throw new UserFriendlyException((int)HttpStatusCode.BadRequest, "Renter has no email.");
            }

            var renterName = booking.Renter?.FullName ?? "Khách hàng";
            var carName = booking.Car != null ? $"{booking.Car.LicensePlate}" : $"#{booking.CarId}";

            await _emailQueue.EnqueueAsync(new BookingEmailJob(
                BookingId: booking.Id,
                RenterId: booking.RenterId,
                RenterEmail: renterEmail,
                RenterName: renterName,
                CarName: carName,
                RentalStart: booking.RentalStart,
                RentalEnd: booking.RentalEnd,
                TotalPriceVnd: booking.TotalPriceVnd,
                PaidAt: DateTime.UtcNow));

            return DataResult.ResultSuccess(true, "Send email queued!");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to queue booking email {Id}", bookingId);
            throw;
        }
    }

    private Task<Booking?> GetEntityByIdAsync(Guid id)
    {
        return _uow.Bookings.Query()
            .Include(b => b.Renter)
            .Include(b => b.Car)
            .Include(b => b.Transactions)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

}

