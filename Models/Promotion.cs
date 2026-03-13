using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class Promotion
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public int DiscountValue { get; set; }

    public PromotionType DiscountType { get; set; }

    public int? MaxDiscountVnd { get; set; }

    public int? MinBookingVnd { get; set; }

    public int? UsageLimit { get; set; }

    public int UsedCount { get; set; }

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
