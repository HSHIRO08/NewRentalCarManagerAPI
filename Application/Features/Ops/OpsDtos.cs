namespace NewRentalCarManagerAPI.Application.Features.Ops;

// ───── Review ─────
public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = null!;
    public Guid RevieweeId { get; set; }
    public string RevieweeName { get; set; } = null!;
    public Guid CarId { get; set; }
    public short Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class CreateReviewDto
{
    public Guid BookingId { get; set; }
    public Guid ReviewerId { get; set; }
    public Guid RevieweeId { get; set; }
    public Guid CarId { get; set; }
    public short Rating { get; set; }
    public string? Comment { get; set; }
}

// ───── DamageReport ─────
public class DamageReportDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid ReportedBy { get; set; }
    public string ReporterName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<string> ImageUrls { get; set; } = new();
    public int? RepairCostVnd { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class CreateDamageReportDto
{
    public Guid BookingId { get; set; }
    public Guid ReportedBy { get; set; }
    public string Description { get; set; } = null!;
    public List<string> ImageUrls { get; set; } = new();
    public int? RepairCostVnd { get; set; }
}
public class UpdateDamageReportDto
{
    public int? RepairCostVnd { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

// ───── Penalty ─────
public class PenaltyDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid ChargedTo { get; set; }
    public string ChargedToName { get; set; } = null!;
    public int AmountVnd { get; set; }
    public string? Description { get; set; }
    public string? EvidenceUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class CreatePenaltyDto
{
    public Guid BookingId { get; set; }
    public Guid ChargedTo { get; set; }
    public int AmountVnd { get; set; }
    public string? Description { get; set; }
    public string? EvidenceUrl { get; set; }
}
