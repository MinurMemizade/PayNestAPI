namespace PayNestAPI.Exceptions
{
    public class CardNotFoundException : Exception
    {
        public CardNotFoundException()
        {
        }

        public CardNotFoundException(string? message) : base(message)
        {
        }
    }
}
