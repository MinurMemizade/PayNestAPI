using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using PayNestAPI.Models.Security;

namespace PayNestAPI.Services.Interfaces
{
    public interface ITokenService
    {
        Task<JwtSecurityToken> CreateToken(AppUser appUser, IList<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
    }
}
