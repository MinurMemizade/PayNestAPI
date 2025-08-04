using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PayNestAPI.AutoMapper.Interfaces;
using PayNestAPI.Exceptions;
using PayNestAPI.Models.DTOs;
using PayNestAPI.Models.Entities;
using PayNestAPI.Models.Security;
using PayNestAPI.Repositories.Interfaces;
using PayNestAPI.Services.Interfaces;
using Stripe;

namespace PayNestAPI.Services.Implementations
{
    public class CardService : ICardService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICardRepository _cardRepository;
        private readonly ICustomMapper _customMapper;
        private readonly UserManager<AppUser> _userManager;

        public CardService(ICardRepository cardRepository, ICustomMapper customMapper, IHttpContextAccessor contextAccessor, UserManager<AppUser> userManager)
        {
            _cardRepository = cardRepository;
            _customMapper = customMapper;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public async Task AddCard(UserCardDTO cardDTO)
        {
            var user = await GetUserClaims();

            long pan = (long)Convert.ToDouble(GenerateNumber(16));
            int cvc = Convert.ToInt32(GenerateNumber(3));

            var map = _customMapper.Map<UserCard, UserCardDTO>(cardDTO);
            map.UserId = user.Id;
            map.PAN = pan;
            map.CVC = cvc;
            map.Expiry = DateTime.Now.AddMonths(40);
            map.Surname = user.Surname;
            map.Name = user.Name;
            map.CardName = user.Name+" "+user.Surname;
            if (cardDTO.Type == Models.Enums.CardType.DEBIT) map.Balance = 0;
            else map.Balance = 300;

                await _cardRepository.AddAsync(map);
        }

        public async Task DeleteCardAsync(Guid cardId)
        {
            await _cardRepository.DeleteAsync(cardId);
        }

        public async Task<List<UserCard>> GetAllCardsAsync()
        {
            var user = await GetUserClaims();
            if (user != null) return await _cardRepository.GetCardsOfUserAsync(user.Id);
            return null;
        }

        public Task<UserCard> GetCardAsync(Guid cardId)
        {
            var loggedIn = _contextAccessor.HttpContext.User;
            if (loggedIn != null) return _cardRepository.GetAsync(cardId);
            return null;
        }

        public async Task IncreaseCardBalanceAsync(Guid Id,double amount)
        {
            var user = await GetUserClaims();
            if (user == null) throw new UserNotFoundException("There is no such user.");

            var cards = await _cardRepository.GetCardsOfUserAsync(user.Id);
            foreach (var card in cards)
            {
                if (card.Id == Id)
                {
                    var service = new PaymentIntentService();
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = (long?)amount * 100,
                        Currency = "usd",
                        Confirm = true,
                        PaymentMethod = "pm_card_visa",
                        PaymentMethodTypes = new List<string> { "card" }
                    };

                    var intent = await service.CreateAsync(options);

                    if (intent.Status == "succeeded")
                    {
                        card.Balance += amount;
                        await _cardRepository.UpdateAsync(card);
                    }
                    else throw new UnableToTopUpException("It is unable to top up your balance.");
                }
            }
            if (cards == null) throw new CardNotFoundException("There is no card.");
        }

        private string GenerateNumber(int k)
        {
            var random = new Random();
            var number = random.Next(1, 10).ToString();
            for (int i = 1; i < k; i++)
            {
                number += random.Next(0, 10);
            }
            return number;
        }

        private async Task<AppUser> GetUserClaims()
        {
            var loggedIn = _contextAccessor.HttpContext.User;
            var user = await _userManager.GetUserAsync(loggedIn);
            return user;
        }
    }
}
