using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayNestAPI.Models.DTOs;
using PayNestAPI.Services.Interfaces;

namespace PayNestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentIntent(PaymentDTO paymentDTO)
        {
            await _paymentService.CreatePaymentIntent(paymentDTO);
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}
