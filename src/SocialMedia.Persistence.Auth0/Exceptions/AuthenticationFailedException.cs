namespace SocialMedia.Persistence.Auth0.Exceptions
{
    public class AuthenticationFailedException : Exception
    {
        private const string MESSAGE = "Unable to login as client.";

        public AuthenticationFailedException() : base(string.Format(MESSAGE)) { }
    }
}
