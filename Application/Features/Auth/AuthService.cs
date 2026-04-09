using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Enums;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Application.Features.Auth;

public interface IAuthService
{
    Task<TokenDto> LoginAsync(LoginDto dto);
    Task<TokenDto> RegisterAsync(RegisterDto dto);
    Task<TokenDto> RefreshAsync(RefreshTokenRequestDto dto);
}

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;

    public AuthService(IUnitOfWork uow, IPasswordHasher hasher, ITokenService tokenService)
    {
        _uow = uow;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    public async Task<TokenDto> LoginAsync(LoginDto dto)
    {
        var user = await _uow.Users.Query()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Phone == dto.Phone)
            ?? throw new ArgumentException("Invalid phone or password");

        if (user.PasswordHash is null || !_hasher.Verify(dto.Password, user.PasswordHash))
            throw new ArgumentException("Invalid phone or password");

        if (user.Status == UserStatus.Banned)
            throw new UnauthorizedAccessException("Account has been banned");

        if (user.Status == UserStatus.Suspended)
            throw new UnauthorizedAccessException("Account is suspended");

        return await GenerateTokensAsync(user);
    }

    public async Task<TokenDto> RegisterAsync(RegisterDto dto)
    {
        var exists = await _uow.Users.Query().AnyAsync(u => u.Phone == dto.Phone);
        if (exists) throw new InvalidOperationException("Phone already registered");

        var defaultRole = await _uow.Roles.Query().FirstOrDefaultAsync(r => r.Name == "renter")
            ?? throw new InvalidOperationException("Default role 'renter' not found");

        var user = new User
        {
            Phone = dto.Phone,
            PasswordHash = _hasher.Hash(dto.Password),
            FullName = dto.FullName,
            Email = dto.Email,
            RoleId = defaultRole.Id,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        user = await _uow.Users.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == user.Id)
            ?? throw new InvalidOperationException("Could not load registered user");
        return await GenerateTokensAsync(user);
    }

    public async Task<TokenDto> RefreshAsync(RefreshTokenRequestDto dto)
    {
        var hash = HashToken(dto.RefreshToken);
        var stored = await _uow.RefreshTokens.Query()
            .Include(r => r.User).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(r => r.TokenHash == hash && r.RevokedAt == null)
            ?? throw new ArgumentException("Invalid refresh token");

        if (stored.ExpiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Refresh token expired");

        stored.RevokedAt = DateTime.UtcNow;

        return await GenerateTokensAsync(stored.User);
    }

    private async Task<TokenDto> GenerateTokensAsync(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Role.Name);
        var refreshTokenRaw = _tokenService.GenerateRefreshToken();

        var refreshEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = HashToken(refreshTokenRaw),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        await _uow.RefreshTokens.AddAsync(refreshEntity);

        return new TokenDto { AccessToken = accessToken, RefreshToken = refreshTokenRaw };
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
