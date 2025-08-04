namespace PayNestAPI.Models.DTOs
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ResetPasswordToken { get; set; }
    }
}
