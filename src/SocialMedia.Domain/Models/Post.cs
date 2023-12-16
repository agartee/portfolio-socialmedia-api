namespace SocialMedia.Domain.Models
{
    public record Post
    {
        public required PostId Id { get; init; }
        public required UserId AuthorUserId { get; init; }
        public required string Text { get; set; }
        public required DateTime Created { get; init; }
    }
}
