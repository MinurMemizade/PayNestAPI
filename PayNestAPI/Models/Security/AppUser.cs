using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using PayNestAPI.Models.Entities;

namespace PayNestAPI.Models.Security
{
    public class AppUser:IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireTime {  get; set; }
        
        [JsonIgnore]
        public List<UserCard> Cards { get; set; }
    }
}
