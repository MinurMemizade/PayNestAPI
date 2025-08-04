using PayNestAPI.Models.DTOs;
using Stripe;

namespace PayNestAPI.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreatePaymentIntent(PaymentDTO paymentDTO);
    }
}
