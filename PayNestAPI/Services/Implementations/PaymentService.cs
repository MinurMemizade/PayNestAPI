using Microsoft.AspNetCore.Identity;
using PayNestAPI.Exceptions;
using PayNestAPI.Models.DTOs;
using PayNestAPI.Models.Security;
using PayNestAPI.Repositories.Interfaces;
using PayNestAPI.Services.Interfaces;
using Stripe;
using Stripe.V2;

namespace PayNestAPI.Services.Implementations
{
    public class PaymentService : IPaymentService
    {

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICardRepository _cardRepository;

        public PaymentService(IHttpContextAccessor contextAccessor, UserManager<AppUser> userManager, ICardRepository cardRepository)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _cardRepository = cardRepository;
        }

        public async Task<PaymentIntent> CreatePaymentIntent(PaymentDTO paymentDTO)
        {
            var options = new PaymentIntentCreateOptions()
            {
                Amount = paymentDTO.Amount*100,
                Currency = paymentDTO.Currency,
                PaymentMethodTypes = new List<string> { $"{paymentDTO.PaymentType}"},
            };

            var claims = _contextAccessor.HttpContext.User;
            var user = await _userManager.GetUserAsync(claims);
            var cards = await _cardRepository.GetCardsOfUserAsync(user.Id);
            foreach (PayNestAPI.Models.Entities.UserCard card in cards)
            {
                if (card.Balance >= (options.Amount/100))
                {
                    card.Balance -= ((double)options.Amount/100);
                    await _cardRepository.UpdateAsync(card);
                }
                else continue;
            }

            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }
    }


}
