using System;
using System.Collections.Generic;

namespace NewRentalCarManagerAPI.Models;

public partial class CarAvailabilityBlock
{
    public Guid Id { get; set; }

    public Guid CarId { get; set; }

    public DateTime BlockedFrom { get; set; }

    public DateTime BlockedTo { get; set; }

    public string Source { get; set; } = null!;

    public Guid? BookingId { get; set; }

    public string? Reason { get; set; }

    public virtual Car Car { get; set; } = null!;
}
