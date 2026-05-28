using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class HandoverRecord
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public HandoverType Type { get; set; }

    public int? MileageKm { get; set; }

    /// <summary>Fuel level 0–100 (%)</summary>
    public int? FuelPercent { get; set; }

    public string? Notes { get; set; }

    public List<string>? ImageUrls { get; set; }

    public Guid RecordedBy { get; set; }

    public DateTime RecordedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual User RecordedByUser { get; set; } = null!;
}
