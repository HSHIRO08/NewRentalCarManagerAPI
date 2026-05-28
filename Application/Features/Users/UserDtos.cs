namespace NewRentalCarManagerAPI.Application.Features.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? ReferralCode { get; set; }
    public Guid? ReferredById { get; set; }
    public string KycStatus { get; set; } = "None";
    public string? IdentityCardUrl { get; set; }
    public string? DriverLicenseUrl { get; set; }
    public string? KycRejectReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateUserDto
{
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public Guid RoleId { get; set; }
    public string? ReferralCode { get; set; }
    public Guid? ReferredById { get; set; }
}

public class UpdateUserDto
{
    public string? Email { get; set; }
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    /// <summary>Optional: Active | Suspended | Banned | Pending</summary>
    public string? Status { get; set; }
}

public class SubmitKycDto
{
    public string IdentityCardUrl { get; set; } = null!;
    public string DriverLicenseUrl { get; set; } = null!;
}

public class ReviewKycDto
{
    /// <summary>approve | reject</summary>
    public string Action { get; set; } = null!;
    public string? RejectReason { get; set; }
}
