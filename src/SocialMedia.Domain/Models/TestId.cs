namespace SocialMedia.Domain.Models
{
    public record TestId : Id<int>
    {
        public TestId(int value) : base(value) { }
    }
}
