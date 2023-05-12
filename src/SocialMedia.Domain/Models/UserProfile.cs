namespace SocialMedia.Domain.Models
{
    public record UserProfile
    {
        public required string Id { get; init; }
        public string? Name { get; init; }
        public string? Nickname { get; init; }
        public string? Email { get; init; }
    }
}
