namespace NewRentalCarManagerAPI.Application.Features.Payments;

public class PaymentListInput
{
    public int SkipCount { get; set; } = 0;
    public int MaxResultCount { get; set; } = 20;
}

// ───── Transaction ─────
public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid PayerId { get; set; }
    public string PayerName { get; set; } = null!;
    public Guid? PayoutId { get; set; }
    public int AmountVnd { get; set; }
    public string? GatewayTxId { get; set; }
    public string? Note { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class CreateTransactionDto
{
    public Guid BookingId { get; set; }
    public Guid PayerId { get; set; }
    public int AmountVnd { get; set; }
    public string? GatewayTxId { get; set; }
    public string? IdempotencyKey { get; set; }
    public string? Note { get; set; }
}

// ───── OwnerPayout ─────
public class OwnerPayoutDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = null!;
    public DateOnly PeriodFrom { get; set; }
    public DateOnly PeriodTo { get; set; }
    public int GrossAmountVnd { get; set; }
    public int PlatformFeeVnd { get; set; }
    public int NetAmountVnd { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? BankAccount { get; set; }
}
public class CreateOwnerPayoutDto
{
    public Guid OwnerId { get; set; }
    public DateOnly PeriodFrom { get; set; }
    public DateOnly PeriodTo { get; set; }
    public string? BankAccount { get; set; }
}

// ───── Deposit ─────
public class DepositStatusDto
{
    public Guid BookingId { get; set; }
    public int DepositVnd { get; set; }
    public bool IsCharged { get; set; }
    public bool IsRefunded { get; set; }
    public int? RefundedAmountVnd { get; set; }
    public Guid? ChargeTransactionId { get; set; }
    public Guid? RefundTransactionId { get; set; }
}

public class RefundDepositDto
{
    /// <summary>Amount to refund. Defaults to full DepositVnd when null.</summary>
    public int? RefundAmountVnd { get; set; }
    public string? Reason { get; set; }
}
