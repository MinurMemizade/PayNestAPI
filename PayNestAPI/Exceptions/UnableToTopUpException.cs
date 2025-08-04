namespace PayNestAPI.Exceptions
{
    public class UnableToTopUpException : Exception
    {
        public UnableToTopUpException()
        {
        }

        public UnableToTopUpException(string? message) : base(message)
        {
        }
    }
}
