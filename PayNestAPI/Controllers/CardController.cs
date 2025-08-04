using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayNestAPI.Models.DTOs;
using PayNestAPI.Services.Interfaces;

namespace PayNestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCardAsync([FromForm]UserCardDTO cardDTO)
        {
            await _cardService.AddCard(cardDTO);
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCardsAsync()
        {
            var result=await _cardService.GetAllCardsAsync();
            return StatusCode(StatusCodes.Status200OK,result);
        }

        [HttpGet("/{Id}")]
        public async Task<IActionResult> GetCardByIdAsync([FromForm] Guid Id)
        {
            var result=await _cardService.GetCardAsync(Id);
            return StatusCode(StatusCodes.Status200OK,result);
        }

        [HttpDelete("/{Id}")]
        public async Task<IActionResult> DeleteCardByIdAsync([FromForm] Guid Id)
        {
            await _cardService.DeleteCardAsync(Id);
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> TopUpBalance(Guid Id, double balance)
        {
            await _cardService.IncreaseCardBalanceAsync(Id, balance);
            return StatusCode(StatusCodes.Status200OK,$"+{balance}");
        }
    }
}
