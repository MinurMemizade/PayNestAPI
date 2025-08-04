using PayNestAPI.Models.DTOs;
using PayNestAPI.Models.Entities;

namespace PayNestAPI.Services.Interfaces
{
    public interface ICardService
    {
        Task<List<UserCard>> GetAllCardsAsync();
        Task<UserCard> GetCardAsync(Guid cardId);
        Task AddCard(UserCardDTO cardDTO);
        Task IncreaseCardBalanceAsync(Guid cardId,double amount);
        Task DeleteCardAsync(Guid cardId);
    }
}
