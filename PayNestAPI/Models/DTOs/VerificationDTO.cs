using System.Text.Json.Serialization;

namespace PayNestAPI.Models.DTOs
{
    public class VerificationDTO
    {
        public string? Email { get; set; }
        public string? VerificationToken { get; set; }

        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }

        [JsonIgnore]
        public DateTime? ExpireDate { get; set; }
    }
}
