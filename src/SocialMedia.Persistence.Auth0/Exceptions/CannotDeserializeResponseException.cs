namespace SocialMedia.Persistence.Auth0.Exceptions
{
    public class CannotDeserializeResponseException : Exception
    {
        private const string MESSAGE = "Unable to deserialize HTTP response message from {0} to {1}.";

        public CannotDeserializeResponseException(string url, Type type) : base(string.Format(MESSAGE, url, type))
        {
        }
    }
}
