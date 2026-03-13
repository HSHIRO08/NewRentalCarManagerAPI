using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class OtpToken
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string Phone { get; set; } = null!;

    public string OtpHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public short AttemptCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public OtpPurpose Purpose { get; set; }

    public virtual User? User { get; set; }
}
