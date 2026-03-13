using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class CarPricing
{
    public Guid Id { get; set; }

    public Guid CarId { get; set; }

    public short? DurationHours { get; set; }

    public int PriceVnd { get; set; }

    public bool IsActive { get; set; }

    public RentalType RentalType { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Car Car { get; set; } = null!;
}
