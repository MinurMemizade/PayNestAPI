namespace PayNestAPI.Exceptions
{
    public class UserNotLoggedInException : Exception
    {
        public UserNotLoggedInException()
        {
        }

        public UserNotLoggedInException(string? message) : base(message)
        {
        }
    }
}
