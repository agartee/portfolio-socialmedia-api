namespace SocialMedia.Domain.Models
{
    public record Id<T> where T : notnull
    {
        protected Id(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
