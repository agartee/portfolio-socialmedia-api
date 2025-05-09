namespace SocialMedia.TestUtilities
{
    public static class MappingContextScope
    {
        private static readonly AsyncLocal<MappingContext?> current = new();

        public static MappingContext Current
        {
            get => current.Value ?? throw new InvalidOperationException("MappingContext has not been initialized.");
            set => current.Value = value;
        }

        public static void Reset() => current.Value = null;
    }
}
