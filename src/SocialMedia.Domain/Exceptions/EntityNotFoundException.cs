namespace SocialMedia.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        private const string MESSAGE = "Unable to find {0} with identifier: {1}";

        public EntityNotFoundException(string typeName, object? id) : base(string.Format(MESSAGE, typeName, id))
        {
        }
    }
}
