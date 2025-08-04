using PayNestAPI.Context;
using PayNestAPI.Models.Entities;
using PayNestAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace PayNestAPI.Repositories.Implementations
{
    public class CardRepository : Repository<UserCard>, ICardRepository
    {
        public CardRepository(AppDBContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<UserCard>> GetAllPendingCardsAsync()
        {
            return await _dbContext.Cards
                .Where(x=>x.State==0)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<UserCard>> GetCardsOfUserAsync(Guid userId)
        {
            return await _dbContext.Cards
                .Where(c=>c.UserId==userId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
