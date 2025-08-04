using PayNestAPI.Models.Entities;

namespace PayNestAPI.Repositories.Interfaces
{
    public interface ICardRepository:IRepository<UserCard>
    {
        Task<List<UserCard>> GetCardsOfUserAsync(Guid userId);
        Task<List<UserCard>> GetAllPendingCardsAsync();
    }
}
