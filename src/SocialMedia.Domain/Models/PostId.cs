namespace SocialMedia.Domain.Models
{
    public record PostId : Id<Guid>
    {
        public PostId(Guid value) : base(value)
        {
            if (value == Guid.Empty)
                throw new InvalidOperationException();
        }

        public static PostId NewId()
        {
            return new PostId(Guid.NewGuid());
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
