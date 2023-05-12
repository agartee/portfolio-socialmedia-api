using System.Diagnostics.CodeAnalysis;

namespace SocialMedia.Domain.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class EntityNotFoundException : Exception
    {
        private const string MESSAGE = "Unable to find {0} with identifier: {1}";

        public EntityNotFoundException(object? id, string typeName) : base(string.Format(MESSAGE, typeName, id))
        {
            Id = id ?? "Unknown";
            TypeName = typeName;
        }

        public object Id { get; }
        public string TypeName { get; }
    }
}
