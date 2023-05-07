namespace SocialMedia.Domain.Exceptions
{
    public class InvalidRequestTypeException : Exception
    {
        private const string MESSAGE = "Type is not a MediatR IRequest: {0}";

        public InvalidRequestTypeException(Type type)
            : base(string.Format(MESSAGE, type))
        {
        }
    }
}
