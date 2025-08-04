using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PayNestAPI.Models.DTOs;
using PayNestAPI.Models.Security;
using PayNestAPI.Services.Interfaces;

namespace PayNestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(IAuthService authService, IEmailService emailService, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _emailService = emailService;
            _userManager = userManager;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> RegisterAsync([FromForm]RegisterDTO registerDTO)
        {
            await _authService.RegisterAsync(registerDTO);
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost("/login")]
        public async Task<IActionResult> LoginAsycn([FromForm] LoginDTO loginDTO)
        {
            var result = await _authService.LoginAsync(loginDTO);
            return StatusCode(StatusCodes.Status200OK,result);
        }

        [HttpPost("confirmEmail")]
        public async Task<IActionResult> SendConfirmationEmail()
        {
            await _emailService.SendConfirmationEmail();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid confirmation request.");
            }

            var owner = await _userManager.FindByIdAsync(userId);
            if (owner == null)
            {
                return BadRequest("User not found.");
            }

            var decodedToken = Uri.UnescapeDataString(token);

            var result = await _userManager.ConfirmEmailAsync(owner, decodedToken);

            if (result.Succeeded)
            {
                return Ok("Email successfully confirmed.");
            }
            else
            {
                return BadRequest("Email confirmation failed.");
            }
        }
    }
}
