namespace PayNestAPI.Models.DTOs
{
    public class PaymentDTO
    {
        public long Amount {  get; set; }
        public string Currency {  get; set; }
        public string PaymentType { get; set; }
    }
}
