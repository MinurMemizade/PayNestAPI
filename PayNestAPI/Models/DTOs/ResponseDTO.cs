namespace PayNestAPI.Models.DTOs
{
    public class ResponseDTO
    {
        public string JWTToken { get; set; }
        public string RefreshToken { get; set; }
        public int StatusCode { get; set; }
        public DateTime? Expiration {  get; set; }
    }
}
