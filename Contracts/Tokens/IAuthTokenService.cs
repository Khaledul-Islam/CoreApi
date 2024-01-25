using System.Security.Claims;

namespace Contracts.Tokens;
public interface IAuthTokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}