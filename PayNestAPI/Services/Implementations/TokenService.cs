using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PayNestAPI.Configurations;
using PayNestAPI.Models.Security;
using PayNestAPI.Services.Interfaces;

namespace PayNestAPI.Services.Implementations
{
    public class TokenService : ITokenService
    {

        private readonly TokenConfigurations _settings;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(UserManager<AppUser> userManager, IOptions<TokenConfigurations> option)
        {
            _userManager = userManager;
            _settings = option.Value;
        }

        public async Task<JwtSecurityToken> CreateToken(AppUser appUser, IList<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,appUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,appUser.Email),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role.ToString()));
            }

            var key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));

            var token = new JwtSecurityToken
                (
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                expires: DateTime.Now.AddMinutes((double)_settings.TokenValidityInMinutes),
                claims: claims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            await _userManager.AddClaimsAsync(appUser, claims);

            return token;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber=new Byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key ?? string.Empty))
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Token not found");
            }
            return principal;
        }
    }
}
