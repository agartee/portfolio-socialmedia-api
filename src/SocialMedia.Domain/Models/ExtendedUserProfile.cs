namespace SocialMedia.Domain.Models
{
    public record ExtendedUserProfile
    {
        public required string UserId { get; init; }
        public string? DisplayName { get; init; }
    }
}
