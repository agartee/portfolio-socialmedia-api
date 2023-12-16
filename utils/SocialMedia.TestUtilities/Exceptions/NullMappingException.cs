using SocialMedia.Domain.Exceptions;

namespace SocialMedia.TestUtilities.Exceptions
{
    public class NullMappingException<TSource, TTarget> : ModelMappingException<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        private const string MESSAGE = "{0} cannot be null.";

        public NullMappingException(string valueName) : base(string.Format(MESSAGE, valueName))
        {
        }
    }
}
