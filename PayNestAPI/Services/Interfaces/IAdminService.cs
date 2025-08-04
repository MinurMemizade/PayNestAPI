using PayNestAPI.Models.Entities;
using PayNestAPI.Models.Security;

namespace PayNestAPI.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<AppUser>> GetAllUsersAsync();
        Task<List<UserCard>> GetAllPendingCards();
        Task ApplyTransactionAsync();
        Task ApplyCardAsync(Guid Id);
    }
}
