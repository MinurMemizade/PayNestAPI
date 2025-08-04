namespace PayNestAPI.Exceptions
{
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException()
        {
        }

        public InsufficientFundsException(string? message) : base(message)
        {
        }
    }
}
