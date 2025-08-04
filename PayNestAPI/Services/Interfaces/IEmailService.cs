using System.Threading.Tasks;
using PayNestAPI.Models.DTOs;

namespace PayNestAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email);
        Task ForgotPasswordAsync(string email);
        Task SendConfirmationEmail();
        Task<bool> VerifyEmail(VerificationDTO verificationDTO);
    }
}
