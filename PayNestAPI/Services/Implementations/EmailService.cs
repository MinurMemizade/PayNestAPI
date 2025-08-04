using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using PayNestAPI.Exceptions;
using PayNestAPI.Models.DTOs;
using PayNestAPI.Models.Security;
using PayNestAPI.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PayNestAPI.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly LinkGenerator _linkGenerator;

        public EmailService(LinkGenerator linkGenerator, UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor, IConfiguration configuration, IMemoryCache cache)
        {
            _linkGenerator = linkGenerator;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
            _cache = cache;
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            var resetCode = new Random().Next(100000, 999999).ToString();

            var cacheKey = $"ResetCode_{user.Email.ToLower()}";
            _cache.Set(cacheKey, resetCode, TimeSpan.FromMinutes(10));

            if (cacheKey == null) throw new Exception();

            var client = new SendGridClient(_configuration["Email:SendGridApiKey"]);
            var from = new EmailAddress(_configuration["Email:SenderEmail"], _configuration["Email:Sender"]);
            var subject = "Password Reset Code";
            var to = new EmailAddress(user.Email);
            var plainTextContent = $"Your password reset code is: {resetCode}";
            var htmlContent = $"<strong>Your password reset code is: {resetCode}</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);
        }

        public async Task SendConfirmationEmail()
        {
            var loggedIn = _contextAccessor.HttpContext.User;
            if (loggedIn == null) throw new UserNotLoggedInException("There is no logged in user.");

            var user = await _userManager.GetUserAsync(loggedIn);
            if (user == null) throw new UserNotFoundException("No user found.");

            await SendVerificationEmailAsync(user.Email);
        }

        public async Task SendVerificationEmailAsync(string email)
        {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    throw new Exception($"User with email '{email}' not found.");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("Failed to generate email confirmation token.");

                var request = _contextAccessor.HttpContext?.Request;
                if (request == null)
                    throw new Exception("Unable to access the HTTP context.");

                string baseUrl = _configuration["AppSettings:BaseUrl"] ?? $"{request.Scheme}://{request.Host.Value}";
                var confirmationLink = _linkGenerator.GetUriByAction(
                    _contextAccessor.HttpContext,
                    action: "ConfirmEmail",
                    controller: "Auth",
                    values: new { userId = user.Id, token = Uri.EscapeDataString(token) },
                    scheme: request.Scheme,
                    host: new HostString(request.Host.Value)
                );

                if (string.IsNullOrEmpty(confirmationLink))
                    throw new Exception("Failed to generate confirmation link.");

                var emailBody = $@"
                <html>
                <body>
                    <h1>Hello!</h1>
                    <p>Thank you for registering with us! To complete your registration, please confirm your email address by clicking the link below:</p>
                    <p><a href='{confirmationLink}' target='_blank'>Verify Email</a></p>
                    <p>If you didn’t register, you can safely ignore this email.</p>
                    <p>Thanks,</p>
                    <p>TourMan Team</p>
                </body>
                </html>";

                var client = new SendGridClient(_configuration["Email:SendGridApiKey"]);
                var from = new EmailAddress(_configuration["Email:SenderEmail"], _configuration["Email:Sender"]);
                var subject = "Confirm Your Email";
                var to = new EmailAddress(email);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, "", emailBody);

                var response = await client.SendEmailAsync(msg);
                var responseBody = await response.Body.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to send email: {response.StatusCode} - {responseBody}");
                }
        }


        public async Task<bool> VerifyEmail(VerificationDTO verificationDTO)
        {
            if (verificationDTO.Email == null || verificationDTO.VerificationToken == null) throw new Exception("Error");
            var user = await _userManager.FindByEmailAsync(verificationDTO.Email);
            if (user == null) throw new Exception("User is null.");
            var result = await _userManager.ConfirmEmailAsync(user, verificationDTO.VerificationToken);
            await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
