using System;
using System.Collections.Generic;
using NewRentalCarManagerAPI.Enums;

namespace NewRentalCarManagerAPI.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string FullName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public Guid RoleId { get; set; }

    public string? ReferralCode { get; set; }

    public Guid? ReferredById { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public UserStatus Status { get; set; }

    public virtual ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();

    public virtual ICollection<ExternalLogin> ExternalLogins { get; set; } = new List<ExternalLogin>();

    public virtual ICollection<User> InverseReferredBy { get; set; } = new List<User>();

    public virtual ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();

    public virtual ICollection<OtpToken> OtpTokens { get; set; } = new List<OtpToken>();

    public virtual ICollection<OwnerPayout> OwnerPayouts { get; set; } = new List<OwnerPayout>();

    public virtual ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();

    public virtual User? ReferredBy { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Review> ReviewReviewees { get; set; } = new List<Review>();

    public virtual ICollection<Review> ReviewReviewers { get; set; } = new List<Review>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
