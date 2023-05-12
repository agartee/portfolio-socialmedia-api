using System.Diagnostics.CodeAnalysis;

namespace SocialMedia.Persistence.Auth0.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class CannotDeserializeResponseException : Exception
    {
        private const string MESSAGE = "Unable to deserialize HTTP response message from {0} to {1}.";

        public CannotDeserializeResponseException(string url, string typeName) : base(string.Format(MESSAGE, typeName, url))
        {
            Url = url;
            TypeName = typeName;
        }

        public string Url { get; }
        public string TypeName { get; }
    }
}
