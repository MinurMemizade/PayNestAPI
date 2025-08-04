using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using PayNestAPI.Exceptions;
using PayNestAPI.Models.DTOs;
using PayNestAPI.Models.Security;
using PayNestAPI.Services.Interfaces;

namespace PayNestAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<Roles> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IFluentEmail _fluentEmail;

        public AuthService(UserManager<AppUser> userManager, RoleManager<Roles> roleManager, IHttpContextAccessor contextAccessor, ITokenService tokenService, IConfiguration configuration, IEmailService emailService, IFluentEmail fluentEmail)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _contextAccessor = contextAccessor;
            _tokenService = tokenService;
            _configuration = configuration;
            _emailService = emailService;
            _fluentEmail = fluentEmail;
        }

        public async Task<ResponseDTO> LoginAsync(LoginDTO loginDTO)
        {
            var oldUser = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (oldUser == null) throw new Exception("Email or Password is incorrect.");

            if (!await _userManager.IsEmailConfirmedAsync(oldUser))
            {
                await _emailService.SendVerificationEmailAsync(oldUser.Email);
                throw new Exception("Email is not verified");
            }

            if (!await _userManager.CheckPasswordAsync(oldUser, loginDTO.Password)) throw new Exception("Email or Password is incorrect.");


            var userRole = await _userManager.GetRolesAsync(oldUser);
            var jwtToken = await _tokenService.CreateToken(oldUser, userRole);
            string refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            oldUser.RefreshToken = refreshToken;
            oldUser.RefreshTokenExpireTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

            await _userManager.UpdateAsync(oldUser);
            await _userManager.UpdateSecurityStampAsync(oldUser);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwtToken);

            await _userManager.SetAuthenticationTokenAsync(oldUser, "Default", "AccessToken", tokenString);


            return new ResponseDTO
            {
                StatusCode = 200,
                JWTToken = tokenString,
                RefreshToken = refreshToken,
                Expiration = jwtToken.ValidTo
            };

        }

        public async Task RegisterAsync(RegisterDTO registerDTO)
        {
            var isExist = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (isExist != null) throw new UserRegisteredException();

            var newUser = new AppUser()
            {
                Name = registerDTO.Name,
                Surname = registerDTO.Surname,
                UserName = (registerDTO.Name+registerDTO.Surname),
                Email = registerDTO.Email,
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, registerDTO.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("admin"))
                {
                    await _roleManager.CreateAsync(new Roles
                    {
                        Id = Guid.NewGuid(),
                        Name = "admin",
                        NormalizedName = "ADMIN",
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                    });
                    await _userManager.AddToRoleAsync(newUser, "admin");
                }

                await _emailService.SendVerificationEmailAsync(newUser.Email);

            }
        }

        public async Task ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _fluentEmail
                .To(user.Email)
                .Subject("Email Verification")
                .Body("Your password refresh token:" + "\n" + token)
                .SendAsync();

            if (!result.Successful) throw new Exception("Failed to send verification email.");
        }

        public async Task ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            ;

            await _userManager.ResetPasswordAsync(user, resetPasswordDTO.ResetPasswordToken, resetPasswordDTO.Password);

        }

        public async Task RevokeAllAsync()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users) 
            {
                if(user.RefreshToken != null)
                {
                    user.RefreshToken = null;
                    await _userManager.UpdateAsync(user);
                }
            }
        }

        public async Task RevokeAsync(string Email)
        {
            var user=await _userManager.FindByEmailAsync(Email);
            if(user != null)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }
        }
    }
}
