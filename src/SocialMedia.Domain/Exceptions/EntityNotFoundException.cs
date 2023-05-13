namespace SocialMedia.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        private const string MESSAGE = "Unable to find {0} with identifier: {1}";

        public EntityNotFoundException(Type type, object? id = null) : base(string.Format(MESSAGE, type.Name, id ?? "unknown"))
        {
        }
    }
}
