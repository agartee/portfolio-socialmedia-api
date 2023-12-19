namespace SocialMedia.Domain.Models
{
    public abstract record Id<T> where T : notnull
    {
        protected Id(T value)
        {
            if (!Id.SupportedTypes.Contains(typeof(T)))
                throw new InvalidOperationException("Invalid backing type");

            Value = value;
        }

        public T Value { get; private set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }

    public static class Id
    {
        public static readonly IEnumerable<Type> SupportedTypes = new List<Type>
        {
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(ulong),
            typeof(decimal),
            typeof(float),
            typeof(double),
            typeof(char),
            typeof(Guid),
            typeof(string)
        };
    }
}
