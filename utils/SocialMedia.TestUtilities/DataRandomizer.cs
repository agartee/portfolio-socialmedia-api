namespace SocialMedia.TestUtilities
{
    public class DataRandomizer<T> where T : notnull
    {
        private readonly Random random;
        private readonly List<T> options = new();

        public DataRandomizer(params T[] options) : this(0, options) { }

        public DataRandomizer(int seed, params T[] options)
        {
            random = new Random(seed);
            this.options.AddRange(options);
        }

        public T Next()
        {
            var idx = random.Next(options.Count);
            var option = options[idx];

            options.Remove(option);
            return option;
        }
    }

    public static class DataRandomizer
    {
        public static DataRandomizer<TItem> Create<TItem>(int seed, params TItem[] items)
            where TItem : notnull
        {
            return new DataRandomizer<TItem>(seed, items);
        }

        public static DataRandomizer<TItem> Create<TItem>(params TItem[] items)
            where TItem : notnull
        {
            return new DataRandomizer<TItem>(items);
        }
    }
}
