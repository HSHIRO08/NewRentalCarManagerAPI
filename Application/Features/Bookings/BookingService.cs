using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Application.Features.Bookings;

// ══════════════════════════════════════════════
//  Booking
// ══════════════════════════════════════════════
public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync();
    Task<BookingDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<BookingDto>> GetByRenterAsync(Guid renterId);
    Task<IEnumerable<BookingDto>> GetByCarAsync(Guid carId);
    Task<BookingDto> CreateAsync(CreateBookingDto dto);
    Task<BookingDto?> UpdateAsync(Guid id, UpdateBookingDto dto);
    Task<bool> DeleteAsync(Guid id);
}

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _uow;
    public BookingService(IUnitOfWork uow) => _uow = uow;

    private IQueryable<Booking> BaseQuery() => _uow.Bookings.Query()
        .Include(b => b.Renter).Include(b => b.Car);

    public async Task<IEnumerable<BookingDto>> GetAllAsync()
    {
        var items = await BaseQuery().OrderByDescending(b => b.CreatedAt).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<BookingDto?> GetByIdAsync(Guid id)
    {
        var e = await BaseQuery().FirstOrDefaultAsync(b => b.Id == id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<IEnumerable<BookingDto>> GetByRenterAsync(Guid renterId)
    {
        var items = await BaseQuery().Where(b => b.RenterId == renterId)
            .OrderByDescending(b => b.CreatedAt).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<BookingDto>> GetByCarAsync(Guid carId)
    {
        var items = await BaseQuery().Where(b => b.CarId == carId)
            .OrderByDescending(b => b.CreatedAt).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<BookingDto> CreateAsync(CreateBookingDto dto)
    {
        var pricing = await _uow.CarPricings.GetByIdAsync(dto.PricingId)
            ?? throw new ArgumentException("Pricing not found");

        var hasOverlap = await _uow.CarAvailabilityBlocks.Query()
            .AnyAsync(b => b.CarId == dto.CarId && b.BlockedFrom < dto.RentalEnd && b.BlockedTo > dto.RentalStart);
        if (hasOverlap) throw new InvalidOperationException("Car is not available for the selected dates");

        var totalHours = (dto.RentalEnd - dto.RentalStart).TotalHours;
        var basePriceVnd = pricing.DurationHours.HasValue && pricing.DurationHours.Value > 0
            ? (int)Math.Ceiling(totalHours / pricing.DurationHours.Value) * pricing.PriceVnd
            : pricing.PriceVnd;

        var discountVnd = 0;
        if (dto.PromotionId.HasValue)
        {
            var promo = await _uow.Promotions.GetByIdAsync(dto.PromotionId.Value);
            if (promo is not null && promo.IsActive
                && DateTime.UtcNow >= promo.ValidFrom && DateTime.UtcNow <= promo.ValidTo
                && (!promo.UsageLimit.HasValue || promo.UsedCount < promo.UsageLimit.Value)
                && (!promo.MinBookingVnd.HasValue || basePriceVnd >= promo.MinBookingVnd.Value))
            {
                discountVnd = promo.MaxDiscountVnd.HasValue
                    ? Math.Min(promo.DiscountValue, promo.MaxDiscountVnd.Value)
                    : promo.DiscountValue;
                promo.UsedCount++;
            }
        }

        var vatVnd = (int)((basePriceVnd - discountVnd) * 0.1);
        var totalPriceVnd = basePriceVnd - discountVnd + vatVnd;

        var entity = new Booking
        {
            RenterId = dto.RenterId, CarId = dto.CarId, PricingId = dto.PricingId,
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

        return (await GetByIdAsync(entity.Id))!;
    }

    public async Task<BookingDto?> UpdateAsync(Guid id, UpdateBookingDto dto)
    {
        var e = await BaseQuery().FirstOrDefaultAsync(b => b.Id == id);
        if (e is null) return null;
        if (dto.ActualReturnAt.HasValue) e.ActualReturnAt = dto.ActualReturnAt;
        if (dto.CancelReason is not null) e.CancelReason = dto.CancelReason;
        if (dto.Note is not null) e.Note = dto.Note;
        e.UpdatedAt = DateTime.UtcNow;
        return MapToDto(e);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var e = await _uow.Bookings.GetByIdAsync(id);
        if (e is null) return false;
        _uow.Bookings.Remove(e);
        return true;
    }

    private static BookingDto MapToDto(Booking e) => new()
    {
        Id = e.Id, RenterId = e.RenterId, RenterName = e.Renter.FullName,
        CarId = e.CarId, CarLicensePlate = e.Car.LicensePlate,
        PricingId = e.PricingId, PromotionId = e.PromotionId,
        PickupLocationId = e.PickupLocationId, ReturnLocationId = e.ReturnLocationId,
        RentalStart = e.RentalStart, RentalEnd = e.RentalEnd, ActualReturnAt = e.ActualReturnAt,
        BasePriceVnd = e.BasePriceVnd, DiscountVnd = e.DiscountVnd,
        SurchargeVnd = e.SurchargeVnd, DeliveryFeeVnd = e.DeliveryFeeVnd,
        TotalPriceVnd = e.TotalPriceVnd, VatVnd = e.VatVnd, DepositVnd = e.DepositVnd,
        CancelReason = e.CancelReason, Note = e.Note,
        CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt
    };
}

// ══════════════════════════════════════════════
//  Promotion
// ══════════════════════════════════════════════
public interface IPromotionService
{
    Task<IEnumerable<PromotionDto>> GetAllAsync();
    Task<PromotionDto?> GetByIdAsync(Guid id);
    Task<PromotionDto?> GetByCodeAsync(string code);
    Task<PromotionDto> CreateAsync(CreatePromotionDto dto);
    Task<PromotionDto?> UpdateAsync(Guid id, UpdatePromotionDto dto);
    Task<bool> DeleteAsync(Guid id);
}

public class PromotionService : IPromotionService
{
    private readonly IUnitOfWork _uow;
    public PromotionService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<PromotionDto>> GetAllAsync()
    {
        var items = await _uow.Promotions.GetAllAsync();
        return items.Select(MapToDto);
    }

    public async Task<PromotionDto?> GetByIdAsync(Guid id)
    {
        var e = await _uow.Promotions.GetByIdAsync(id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<PromotionDto?> GetByCodeAsync(string code)
    {
        var e = await _uow.Promotions.Query().FirstOrDefaultAsync(p => p.Code == code && p.IsActive);
        return e is null ? null : MapToDto(e);
    }

    public async Task<PromotionDto> CreateAsync(CreatePromotionDto dto)
    {
        var e = new Promotion
        {
            Code = dto.Code, DiscountValue = dto.DiscountValue,
            MaxDiscountVnd = dto.MaxDiscountVnd, MinBookingVnd = dto.MinBookingVnd,
            UsageLimit = dto.UsageLimit, UsedCount = 0,
            ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo, IsActive = true
        };
        await _uow.Promotions.AddAsync(e);
        return MapToDto(e);
    }

    public async Task<PromotionDto?> UpdateAsync(Guid id, UpdatePromotionDto dto)
    {
        var e = await _uow.Promotions.GetByIdAsync(id);
        if (e is null) return null;
        e.DiscountValue = dto.DiscountValue; e.MaxDiscountVnd = dto.MaxDiscountVnd;
        e.MinBookingVnd = dto.MinBookingVnd; e.UsageLimit = dto.UsageLimit;
        e.ValidFrom = dto.ValidFrom; e.ValidTo = dto.ValidTo; e.IsActive = dto.IsActive;
        return MapToDto(e);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var e = await _uow.Promotions.GetByIdAsync(id);
        if (e is null) return false;
        _uow.Promotions.Remove(e);
        return true;
    }

    private static PromotionDto MapToDto(Promotion e) => new()
    {
        Id = e.Id, Code = e.Code, DiscountValue = e.DiscountValue,
        MaxDiscountVnd = e.MaxDiscountVnd, MinBookingVnd = e.MinBookingVnd,
        UsageLimit = e.UsageLimit, UsedCount = e.UsedCount,
        ValidFrom = e.ValidFrom, ValidTo = e.ValidTo, IsActive = e.IsActive
    };
}
