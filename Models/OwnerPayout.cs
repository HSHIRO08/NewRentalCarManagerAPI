using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class OwnerPayout
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public DateOnly PeriodFrom { get; set; }

    public DateOnly PeriodTo { get; set; }

    public int GrossAmountVnd { get; set; }

    public int PlatformFeeVnd { get; set; }

    public int NetAmountVnd { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? BankAccount { get; set; }

    public PaymentStatus Status { get; set; }

    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
