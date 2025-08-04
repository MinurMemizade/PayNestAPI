namespace PayNestAPI.Exceptions
{
    public class UserRegisteredException : Exception
    {
        public UserRegisteredException()
        {
        }

        public UserRegisteredException(string? message) : base(message)
        {
        }
    }
}
