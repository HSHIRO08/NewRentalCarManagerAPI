using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Application.Features.Ops;

// ══════════════════════════════════════════════
//  Review
// ══════════════════════════════════════════════
public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetByBookingAsync(Guid bookingId);
    Task<IEnumerable<ReviewDto>> GetByCarAsync(Guid carId);
    Task<ReviewDto?> GetByIdAsync(Guid id);
    Task<ReviewDto> CreateAsync(CreateReviewDto dto);
    Task<bool> DeleteAsync(Guid id);
}

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _uow;
    public ReviewService(IUnitOfWork uow) => _uow = uow;

    private IQueryable<Review> BaseQuery() => _uow.Reviews.Query()
        .Include(r => r.Reviewer).Include(r => r.Reviewee);

    public async Task<IEnumerable<ReviewDto>> GetByBookingAsync(Guid bookingId)
    {
        var items = await BaseQuery().Where(r => r.BookingId == bookingId).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<ReviewDto>> GetByCarAsync(Guid carId)
    {
        var items = await BaseQuery().Where(r => r.CarId == carId)
            .OrderByDescending(r => r.CreatedAt).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<ReviewDto?> GetByIdAsync(Guid id)
    {
        var e = await BaseQuery().FirstOrDefaultAsync(r => r.Id == id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<ReviewDto> CreateAsync(CreateReviewDto dto)
    {
        var e = new Review
        {
            BookingId = dto.BookingId, ReviewerId = dto.ReviewerId,
            RevieweeId = dto.RevieweeId, CarId = dto.CarId,
            Rating = dto.Rating, Comment = dto.Comment, CreatedAt = DateTime.UtcNow
        };
        await _uow.Reviews.AddAsync(e);
        return (await GetByIdAsync(e.Id))!;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var e = await _uow.Reviews.GetByIdAsync(id);
        if (e is null) return false;
        _uow.Reviews.Remove(e);
        return true;
    }

    private static ReviewDto MapToDto(Review e) => new()
    {
        Id = e.Id, BookingId = e.BookingId,
        ReviewerId = e.ReviewerId, ReviewerName = e.Reviewer.FullName,
        RevieweeId = e.RevieweeId, RevieweeName = e.Reviewee.FullName,
        CarId = e.CarId, Rating = e.Rating, Comment = e.Comment, CreatedAt = e.CreatedAt
    };
}

// ══════════════════════════════════════════════
//  DamageReport
// ══════════════════════════════════════════════
public interface IDamageReportService
{
    Task<IEnumerable<DamageReportDto>> GetByBookingAsync(Guid bookingId);
    Task<DamageReportDto?> GetByIdAsync(Guid id);
    Task<DamageReportDto> CreateAsync(CreateDamageReportDto dto);
    Task<DamageReportDto?> UpdateAsync(Guid id, UpdateDamageReportDto dto);
}

public class DamageReportService : IDamageReportService
{
    private readonly IUnitOfWork _uow;
    public DamageReportService(IUnitOfWork uow) => _uow = uow;

    private IQueryable<DamageReport> BaseQuery() => _uow.DamageReports.Query()
        .Include(d => d.ReportedByNavigation);

    public async Task<IEnumerable<DamageReportDto>> GetByBookingAsync(Guid bookingId)
    {
        var items = await BaseQuery().Where(d => d.BookingId == bookingId).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<DamageReportDto?> GetByIdAsync(Guid id)
    {
        var e = await BaseQuery().FirstOrDefaultAsync(d => d.Id == id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<DamageReportDto> CreateAsync(CreateDamageReportDto dto)
    {
        var e = new DamageReport
        {
            BookingId = dto.BookingId, ReportedBy = dto.ReportedBy,
            Description = dto.Description, ImageUrls = dto.ImageUrls,
            RepairCostVnd = dto.RepairCostVnd, CreatedAt = DateTime.UtcNow
        };
        await _uow.DamageReports.AddAsync(e);
        return (await GetByIdAsync(e.Id))!;
    }

    public async Task<DamageReportDto?> UpdateAsync(Guid id, UpdateDamageReportDto dto)
    {
        var e = await BaseQuery().FirstOrDefaultAsync(d => d.Id == id);
        if (e is null) return null;
        e.RepairCostVnd = dto.RepairCostVnd; e.ResolvedAt = dto.ResolvedAt;
        return MapToDto(e);
    }

    private static DamageReportDto MapToDto(DamageReport e) => new()
    {
        Id = e.Id, BookingId = e.BookingId, ReportedBy = e.ReportedBy,
        ReporterName = e.ReportedByNavigation.FullName,
        Description = e.Description, ImageUrls = e.ImageUrls,
        RepairCostVnd = e.RepairCostVnd, ResolvedAt = e.ResolvedAt, CreatedAt = e.CreatedAt
    };
}

// ══════════════════════════════════════════════
//  Penalty
// ══════════════════════════════════════════════
public interface IPenaltyService
{
    Task<IEnumerable<PenaltyDto>> GetByBookingAsync(Guid bookingId);
    Task<PenaltyDto?> GetByIdAsync(Guid id);
    Task<PenaltyDto> CreateAsync(CreatePenaltyDto dto);
}

public class PenaltyService : IPenaltyService
{
    private readonly IUnitOfWork _uow;
    public PenaltyService(IUnitOfWork uow) => _uow = uow;

    private IQueryable<Penalty> BaseQuery() => _uow.Penalties.Query()
        .Include(p => p.ChargedToNavigation);

    public async Task<IEnumerable<PenaltyDto>> GetByBookingAsync(Guid bookingId)
    {
        var items = await BaseQuery().Where(p => p.BookingId == bookingId).ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<PenaltyDto?> GetByIdAsync(Guid id)
    {
        var e = await BaseQuery().FirstOrDefaultAsync(p => p.Id == id);
        return e is null ? null : MapToDto(e);
    }

    public async Task<PenaltyDto> CreateAsync(CreatePenaltyDto dto)
    {
        var e = new Penalty
        {
            BookingId = dto.BookingId, ChargedTo = dto.ChargedTo,
            AmountVnd = dto.AmountVnd, Description = dto.Description,
            EvidenceUrl = dto.EvidenceUrl, CreatedAt = DateTime.UtcNow
        };
        await _uow.Penalties.AddAsync(e);
        return (await GetByIdAsync(e.Id))!;
    }

    private static PenaltyDto MapToDto(Penalty e) => new()
    {
        Id = e.Id, BookingId = e.BookingId, ChargedTo = e.ChargedTo,
        ChargedToName = e.ChargedToNavigation.FullName,
        AmountVnd = e.AmountVnd, Description = e.Description,
        EvidenceUrl = e.EvidenceUrl, CreatedAt = e.CreatedAt
    };
}
