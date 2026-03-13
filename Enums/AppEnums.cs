using NpgsqlTypes;

namespace NewRentalCarManagerAPI.Enums;

public enum BookingStatus
{
    Pending = 0,
    Confirmed = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4,
    Disputed = 5
}

public enum PromotionType
{
    Percentage = 0,
    [PgName("fixed_amount")]
    FixedAmount = 1
}

public enum SagaStatus
{
    Started = 0,
    Succeeded = 1,
    Failed = 2,
    Compensating = 3
}

public enum CarStatus
{
    Available = 0,
    Rented = 1,
    Maintenance = 2,
    Inactive = 3
}

public enum FuelType
{
    Gasoline = 0,
    Diesel = 1,
    Electric = 2,
    Hybrid = 3
}

public enum RentalType
{
    Hourly = 0,
    Daily = 1
}

public enum TransmissionType
{
    Automatic = 0,
    Manual = 1
}

public enum OtpPurpose
{
    Login = 0,
    Register = 1,
    [PgName("reset_password")]
    ResetPassword = 2,
    [PgName("verify_phone")]
    VerifyPhone = 3
}

public enum ProviderName
{
    Google = 0,
    Facebook = 1,
    Zalo = 2,
    Apple = 3
}

public enum UserStatus
{
    Pending = 0,
    Active = 1,
    Suspended = 2,
    Banned = 3
}

public enum NotifChannel
{
    Push = 0,
    Sms = 1,
    Email = 2,
    [PgName("in_app")]
    InApp = 3
}

public enum NotifStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2,
    Skipped = 3
}

public enum DamageSeverity
{
    Minor = 0,
    Moderate = 1,
    Severe = 2
}

public enum DamageStatus
{
    Reported = 0,
    Assessed = 1,
    Resolved = 2,
    Disputed = 3
}

public enum PenaltyStatus
{
    Pending = 0,
    Charged = 1,
    Disputed = 2,
    Waived = 3
}

public enum PenaltyType
{
    [PgName("traffic_fine")]
    TrafficFine = 0,
    [PgName("toll_violation")]
    TollViolation = 1,
    [PgName("late_return")]
    LateReturn = 2,
    [PgName("fuel_shortage")]
    FuelShortage = 3,
    [PgName("cleaning_fee")]
    CleaningFee = 4
}

public enum PaymentDirection
{
    Charge = 0,
    Refund = 1,
    Payout = 2
}

public enum PaymentMethod
{
    [PgName("bank_transfer")]
    BankTransfer = 0,
    Momo = 1,
    [PgName("vnpay")]
    VnPay = 2,
    [PgName("zalopay")]
    ZaloPay = 3,
    [PgName("credit_card")]
    CreditCard = 4
}

public enum PaymentStatus
{
    Pending = 0,
    Success = 1,
    Failed = 2,
    Refunded = 3
}
