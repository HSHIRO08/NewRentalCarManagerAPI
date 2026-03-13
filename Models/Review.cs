using System;
using System.Collections.Generic;

namespace NewRentalCarManagerAPI.Models;

public partial class Review
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public Guid ReviewerId { get; set; }

    public Guid RevieweeId { get; set; }

    public Guid CarId { get; set; }

    public short Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Car Car { get; set; } = null!;

    public virtual User Reviewee { get; set; } = null!;

    public virtual User Reviewer { get; set; } = null!;
}
