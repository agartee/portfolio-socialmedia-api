namespace SocialMedia.Domain.Models
{
    public record UserId : Id<string>
    {
        public UserId(string value) : base(value)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
