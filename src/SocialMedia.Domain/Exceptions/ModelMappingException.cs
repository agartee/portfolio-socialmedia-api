namespace SocialMedia.Domain.Exceptions
{
    public class ModelMappingException<TSource, TTarget> : Exception
        where TSource : class
        where TTarget : class
    {
        private const string MESSAGE = "Unable to map {0} to {1}. {2}";

        public ModelMappingException(string reason)
            : base(string.Format(MESSAGE, typeof(TSource).Name, typeof(TTarget).Name, reason))
        {
        }
    }
}
