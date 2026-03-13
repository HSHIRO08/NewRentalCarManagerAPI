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
    public string? ReferralCode { get; set; }
    public Guid? ReferredById { get; set; }
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
}
