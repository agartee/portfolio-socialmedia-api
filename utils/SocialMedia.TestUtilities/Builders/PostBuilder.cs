using SocialMedia.Domain.Models;
using SocialMedia.TestUtilities.Models;

namespace SocialMedia.TestUtilities.Builders
{
    public class PostBuilder
    {
        private readonly DataRandomizer<string> textRandomizer = DataRandomizer.Create(
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
            "Justo laoreet sit amet cursus. Et tortor at risus viverra.",
            "Risus quis varius quam quisque id diam.",
            "Vel facilisis volutpat est velit egestas dui id ornare arcu.",
            "Netus et malesuada fames ac turpis egestas integer.",
            "Quis commodo odio aenean sed adipiscing diam donec adipiscing.");

        private readonly UserBuilder userBuilder = new();

        public PostConfiguration CreatePost()
        {
            return new PostConfiguration()
                .WithId(PostId.NewId())
                .WithAuthor(userBuilder.CreateUser())
                .WithText(textRandomizer.Next())
                .WithCreated(DateTime.UtcNow);
        }

        private record AuthorData
        {
            public required string UserId { get; init; }
            public required string Name { get; init; }
        }
    }
}
