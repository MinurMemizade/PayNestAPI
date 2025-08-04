using PayNestAPI.Models.DTOs;

namespace PayNestAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDTO registerDTO);
        Task<ResponseDTO> LoginAsync(LoginDTO loginDTO);
        Task RevokeAsync(string Email);
        Task RevokeAllAsync();
    }
}
