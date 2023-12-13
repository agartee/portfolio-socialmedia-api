namespace SocialMedia.Domain.Models
{
    public record User
    {
        public required UserId Id { get; init; }
        public required string Name { get; init; }
    }
}
