namespace SocialMedia.TestUtilities.Exceptions
{
    public class NullMappingException<TSource> : Exception
    where TSource : class
    {
        public string PropertyName { get; }

        public NullMappingException(string propertyName)
            : base($"{typeof(TSource).Name}.{propertyName} must not be null during mapping.")
        {
            PropertyName = propertyName;
        }
    }

}
