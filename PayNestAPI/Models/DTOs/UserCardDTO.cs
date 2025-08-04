using System.ComponentModel.DataAnnotations;
using PayNestAPI.Models.Enums;
using PayNestAPI.Models.Security;

namespace PayNestAPI.Models.DTOs
{
    public class UserCardDTO
    {
        [Range (1,2)]
        public CardType Type { get; set; }
    }
}
