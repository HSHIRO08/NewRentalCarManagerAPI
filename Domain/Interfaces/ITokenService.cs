namespace NewRentalCarManagerAPI.Domain.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string role);
    string GenerateRefreshToken();
}
