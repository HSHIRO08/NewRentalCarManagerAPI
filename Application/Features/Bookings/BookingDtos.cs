namespace NewRentalCarManagerAPI.Application.Features.Bookings;

// ───── Booking ─────
public class BookingDto
{
    public Guid Id { get; set; }
    public Guid RenterId { get; set; }
    public string RenterName { get; set; } = null!;
    public Guid CarId { get; set; }
    public string CarLicensePlate { get; set; } = null!;
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Frontend-friendly aliases
    public string Status { get; set; } = null!;
    public string PaymentStatus { get; set; } = "Pending";
    public string EmailStatus { get; set; } = "Pending";
    public string StartDate => RentalStart.ToString("yyyy-MM-dd");
    public string EndDate => RentalEnd.ToString("yyyy-MM-dd");
    public int TotalPrice => TotalPriceVnd;
}
public class CreateBookingDto
{
    public Guid RenterId { get; set; }
    public Guid CarId { get; set; }
    public Guid PricingId { get; set; }
    public Guid? PromotionId { get; set; }
    public Guid PickupLocationId { get; set; }
    public Guid ReturnLocationId { get; set; }
    public DateTime RentalStart { get; set; }
    public DateTime RentalEnd { get; set; }
    public int DepositVnd { get; set; }
    public string? Note { get; set; }
}
public class QuickCreateBookingDto
{
    public Guid CarId { get; set; }
    public string StartDate { get; set; } = null!;
    public string EndDate { get; set; } = null!;
    public string? Note { get; set; }
}
public class UpdateBookingDto
{
    public string? Status { get; set; }
    public DateTime? ActualReturnAt { get; set; }
    public string? CancelReason { get; set; }
    public string? Note { get; set; }
}

// ───── Promotion ─────
public class PromotionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public int DiscountValue { get; set; }
    public int? MaxDiscountVnd { get; set; }
    public int? MinBookingVnd { get; set; }
    public int? UsageLimit { get; set; }
    public int UsedCount { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; }
}
public class CreatePromotionDto
{
    public string Code { get; set; } = null!;
    public int DiscountValue { get; set; }
    public int? MaxDiscountVnd { get; set; }
    public int? MinBookingVnd { get; set; }
    public int? UsageLimit { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}
public class UpdatePromotionDto
{
    public int DiscountValue { get; set; }
    public int? MaxDiscountVnd { get; set; }
    public int? MinBookingVnd { get; set; }
    public int? UsageLimit { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; }
}
