using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class DamageReport
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public Guid ReportedBy { get; set; }

    public string Description { get; set; } = null!;

    public List<string> ImageUrls { get; set; } = null!;

    public int? RepairCostVnd { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DamageSeverity Severity { get; set; }

    public DamageStatus Status { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual User ReportedByNavigation { get; set; } = null!;
}
