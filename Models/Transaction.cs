using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public Guid PayerId { get; set; }

    public Guid? PayoutId { get; set; }

    public int AmountVnd { get; set; }

    public string? GatewayTxId { get; set; }

    public string? GatewayMeta { get; set; }

    public string? IdempotencyKey { get; set; }

    public string? Note { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public PaymentDirection Direction { get; set; }

    public PaymentMethod Method { get; set; }

    public PaymentStatus Status { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual User Payer { get; set; } = null!;

    public virtual OwnerPayout? Payout { get; set; }
}
