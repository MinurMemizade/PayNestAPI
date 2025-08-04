using System.Text.Json.Serialization;
using PayNestAPI.Models.Common;
using PayNestAPI.Models.Enums;
using PayNestAPI.Models.Security;

namespace PayNestAPI.Models.Entities
{
    public class UserCard:BaseEntity
    {
        public long PAN { get; set; }
        public int CVC { get; set; }
        public string CardName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public CardType Type { get; set; }
        public CardStates State { get; set; }
        public DateTime Expiry { get; set; }
        public double Balance { get; set; }
        public Guid UserId { get; set; }

        [JsonIgnore]
        public AppUser User { get; set; }
    }
}
