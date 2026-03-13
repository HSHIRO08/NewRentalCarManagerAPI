using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class Booking
{
    public Guid Id { get; set; }

    public Guid RenterId { get; set; }

    public Guid CarId { get; set; }

    public Guid PricingId { get; set; }

    public Guid? PromotionId { get; set; }

    public Guid PickupLocationId { get; set; }

    public Guid ReturnLocationId { get; set; }

    public DateTime RentalStart { get; set; }

    public DateTime RentalEnd { get; set; }

    public DateTime? ActualReturnAt { get; set; }

    public int BasePriceVnd { get; set; }

    public int DiscountVnd { get; set; }

    public int SurchargeVnd { get; set; }

    public int DeliveryFeeVnd { get; set; }

    public int TotalPriceVnd { get; set; }

    public int VatVnd { get; set; }

    public int DepositVnd { get; set; }

    public string? CancelReason { get; set; }

    public string? Note { get; set; }

    public string? SagaStep { get; set; }

    public string? SagaContext { get; set; }

    public BookingStatus Status { get; set; }

    public SagaStatus SagaStatusEnum { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();

    public virtual ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();

    public virtual Location PickupLocation { get; set; } = null!;

    public virtual CarPricing Pricing { get; set; } = null!;

    public virtual Promotion? Promotion { get; set; }

    public virtual User Renter { get; set; } = null!;

    public virtual Location ReturnLocation { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
