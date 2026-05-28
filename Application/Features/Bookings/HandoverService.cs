using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Enums;
using NewRentalCarManagerAPI.Models;
using System.Net;

namespace NewRentalCarManagerAPI.Application.Features.Bookings;

// ════════════════════════════════
//  DTOs
// ════════════════════════════════

public class HandoverDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string Type { get; set; } = null!;
    public int? MileageKm { get; set; }
    public int? FuelPercent { get; set; }
    public string? Notes { get; set; }
    public List<string>? ImageUrls { get; set; }
    public string RecordedByName { get; set; } = null!;
    public DateTime RecordedAt { get; set; }
}

public class CheckInDto
{
    public int? MileageKm { get; set; }
    /// <summary>0–100 (%)</summary>
    public int? FuelPercent { get; set; }
    public string? Notes { get; set; }
    public List<string>? ImageUrls { get; set; }
}

public class CheckOutDto
{
    public int? MileageKm { get; set; }
    /// <summary>0–100 (%)</summary>
    public int? FuelPercent { get; set; }
    public string? Notes { get; set; }
    public List<string>? ImageUrls { get; set; }
}

// ════════════════════════════════
//  Interface + Service
// ════════════════════════════════

public interface IHandoverService
{
    Task<DataResult> GetByBookingAsync(Guid bookingId);
    Task<DataResult> CheckInAsync(Guid bookingId, Guid actorId, CheckInDto dto);
    Task<DataResult> CheckOutAsync(Guid bookingId, Guid actorId, CheckOutDto dto);
}

public class HandoverService : IHandoverService
{
    private readonly IRepository<HandoverRecord> _handoverRecordRepository;
    private readonly IRepository<Booking> _bookingRepository;
    private readonly IRepository<Car> _carRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<HandoverService> _logger;

    public HandoverService(IRepository<HandoverRecord> handoverRecordRepository, IRepository<Booking> bookingRepository, IRepository<Car> carRepository, IMapper mapper, ILogger<HandoverService> logger)
    {
        _handoverRecordRepository = handoverRecordRepository;
        _bookingRepository = bookingRepository;
        _carRepository = carRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DataResult> GetByBookingAsync(Guid bookingId)
    {
        try
        {
            var list = await _handoverRecordRepository.Query()
                .Include(h => h.RecordedByUser)
                .Where(h => h.BookingId == bookingId)
                .OrderBy(h => h.RecordedAt)
                .ProjectTo<HandoverDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return DataResult.ResultSuccess(list, "Success");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get handover records for booking {BookingId}", bookingId);
            throw;
        }
    }

    public async Task<DataResult> CheckInAsync(Guid bookingId, Guid actorId, CheckInDto dto)
    {
        try
        {
            var booking = await _bookingRepository.Query()
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.Id == bookingId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Booking not found!");

            if (booking.Status != BookingStatus.Confirmed)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict,
                    $"Check-in requires booking status 'Confirmed'. Current: {booking.Status}");

            var alreadyCheckedIn = await _handoverRecordRepository.Query()
                .AnyAsync(h => h.BookingId == bookingId && h.Type == HandoverType.CheckIn);
            if (alreadyCheckedIn)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict, "Check-in đã được thực hiện cho booking này.");

            // Validate fuel percent
            if (dto.FuelPercent is < 0 or > 100)
                throw new UserFriendlyException((int)HttpStatusCode.BadRequest, "FuelPercent phải trong khoảng 0–100.");

            var record = new HandoverRecord
            {
                BookingId = bookingId,
                Type = HandoverType.CheckIn,
                MileageKm = dto.MileageKm,
                FuelPercent = dto.FuelPercent,
                Notes = dto.Notes,
                ImageUrls = dto.ImageUrls,
                RecordedBy = actorId,
                RecordedAt = DateTime.UtcNow
            };
            await _handoverRecordRepository.AddAsync(record);

            // Transition booking: Confirmed → Active
            booking.Status = BookingStatus.Active;
            booking.UpdatedAt = DateTime.UtcNow;

            // Mark car as Rented
            booking.Car.Status = CarStatus.Rented;

            var result = await _handoverRecordRepository.Query()
                .Include(h => h.RecordedByUser)
                .FirstOrDefaultAsync(h => h.Id == record.Id);
            return DataResult.ResultSuccess(_mapper.Map<HandoverDto>(result), "Check-in thành công!", statusCode: 201);
        }
        catch (Exception e)
        {
            if (e is not UserFriendlyException)
                _logger.LogError(e, "Failed to check-in booking {BookingId}", bookingId);
            throw;
        }
    }

    public async Task<DataResult> CheckOutAsync(Guid bookingId, Guid actorId, CheckOutDto dto)
    {
        try
        {
            var booking = await _bookingRepository.Query()
                .Include(b => b.Car)
                .Include(b => b.Pricing)
                .FirstOrDefaultAsync(b => b.Id == bookingId)
                ?? throw new UserFriendlyException((int)HttpStatusCode.NotFound, "Booking not found!");

            if (booking.Status != BookingStatus.Active)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict,
                    $"Check-out requires booking status 'Active'. Current: {booking.Status}");

            var alreadyCheckedOut = await _handoverRecordRepository.Query()
                .AnyAsync(h => h.BookingId == bookingId && h.Type == HandoverType.CheckOut);
            if (alreadyCheckedOut)
                throw new UserFriendlyException((int)HttpStatusCode.Conflict, "Check-out đã được thực hiện cho booking này.");

            if (dto.FuelPercent is < 0 or > 100)
                throw new UserFriendlyException((int)HttpStatusCode.BadRequest, "FuelPercent phải trong khoảng 0–100.");

            var actualReturn = DateTime.UtcNow;

            // Compute late-return surcharge
            int lateFeVnd = 0;
            if (actualReturn > booking.RentalEnd && booking.Pricing != null)
            {
                var overtime = actualReturn - booking.RentalEnd;
                if (booking.Pricing.RentalType == RentalType.Hourly)
                {
                    // PriceVnd = price per block of DurationHours; compute per-hour rate
                    var hoursPerBlock = booking.Pricing.DurationHours ?? 1;
                    var pricePerHour = hoursPerBlock > 0
                        ? booking.Pricing.PriceVnd / (double)hoursPerBlock
                        : booking.Pricing.PriceVnd;
                    var lateHours = Math.Ceiling(overtime.TotalHours);
                    lateFeVnd = (int)(pricePerHour * lateHours);
                }
                else
                {
                    // Daily: PriceVnd = price per day
                    var lateDays = Math.Ceiling(overtime.TotalHours / 24.0);
                    lateFeVnd = (int)(booking.Pricing.PriceVnd * lateDays);
                }
            }

            var record = new HandoverRecord
            {
                BookingId = bookingId,
                Type = HandoverType.CheckOut,
                MileageKm = dto.MileageKm,
                FuelPercent = dto.FuelPercent,
                Notes = dto.Notes,
                ImageUrls = dto.ImageUrls,
                RecordedBy = actorId,
                RecordedAt = actualReturn
            };
            await _handoverRecordRepository.AddAsync(record);

            // Apply surcharge and update booking
            booking.ActualReturnAt = actualReturn;
            booking.SurchargeVnd += lateFeVnd;
            booking.TotalPriceVnd = booking.BasePriceVnd - booking.DiscountVnd + booking.VatVnd
                                    + booking.SurchargeVnd + booking.DeliveryFeeVnd;
            booking.Status = BookingStatus.Completed;
            booking.UpdatedAt = actualReturn;

            // Release car back to Available
            booking.Car.Status = CarStatus.Available;

            var result = await _handoverRecordRepository.Query()
                .Include(h => h.RecordedByUser)
                .FirstOrDefaultAsync(h => h.Id == record.Id);
            return DataResult.ResultSuccess(_mapper.Map<HandoverDto>(result), "Check-out thành công!" +
                (lateFeVnd > 0 ? $" Phí trễ hạn: {lateFeVnd:N0} VND." : ""), statusCode: 201);
        }
        catch (Exception e)
        {
            if (e is not UserFriendlyException)
                _logger.LogError(e, "Failed to check-out booking {BookingId}", bookingId);
            throw;
        }
    }
}
