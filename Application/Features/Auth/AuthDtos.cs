namespace NewRentalCarManagerAPI.Application.Features.Auth;

public class LoginDto
{
    public string Phone { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegisterDto
{
    public string Phone { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
}

public class TokenDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}

public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = null!;
}
