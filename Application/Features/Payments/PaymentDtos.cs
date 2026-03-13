namespace NewRentalCarManagerAPI.Application.Features.Payments;

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
    public int GrossAmountVnd { get; set; }
    public int PlatformFeeVnd { get; set; }
    public string? BankAccount { get; set; }
}
