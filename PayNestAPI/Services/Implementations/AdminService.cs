using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PayNestAPI.Exceptions;
using PayNestAPI.Models.Entities;
using PayNestAPI.Models.Security;
using PayNestAPI.Repositories.Interfaces;
using PayNestAPI.Services.Interfaces;

namespace PayNestAPI.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICardRepository _cardRepository;

        public AdminService(UserManager<AppUser> userManager, ICardRepository cardRepository)
        {
            _userManager = userManager;
            _cardRepository = cardRepository;
        }

        public async Task ApplyCardAsync(Guid Id)
        {
            var card = await _cardRepository.GetAsync(Id);
            if (card == null) throw new CardNotFoundException();

            card.State = Models.Enums.CardStates.ACTIVE;
            await _cardRepository.UpdateAsync(card);
        }

        public Task ApplyTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserCard>> GetAllPendingCards()
        {
            return await _cardRepository.GetAllPendingCardsAsync();
        }

        public async Task<List<AppUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }
    }
}
