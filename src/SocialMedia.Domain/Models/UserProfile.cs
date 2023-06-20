namespace SocialMedia.Domain.Models
{
    public record UserProfile
    {
        public required string UserId { get; init; }
        public required string Name { get; init; }
        public required string Email { get; init; }
    }
}
