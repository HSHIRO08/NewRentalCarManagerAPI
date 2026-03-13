using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class Penalty
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public Guid ChargedTo { get; set; }

    public int AmountVnd { get; set; }

    public string? Description { get; set; }

    public string? EvidenceUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public PenaltyType Type { get; set; }

    public PenaltyStatus Status { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual User ChargedToNavigation { get; set; } = null!;
}
