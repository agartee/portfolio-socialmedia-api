using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Models;
using SocialMedia.TestUtilities.Exceptions;

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

    public record PostConfiguration
    {
        public PostId? Id { get; private set; }
        public UserConfiguration? Author { get; private set; }
        public string? Text { get; private set; }
        public DateTime? Created { get; private set; }

        public PostConfiguration WithId(PostId? id)
        {
            Id = id;
            return this;
        }

        public PostConfiguration WithAuthor(UserConfiguration? author)
        {
            Author = author;
            return this;
        }

        public PostConfiguration WithText(string? text)
        {
            Text = text;
            return this;
        }

        public PostConfiguration WithCreated(DateTime? created)
        {
            Created = created;
            return this;
        }

        public Post ToPost()
        {
            return new Post
            {
                Id = Id ?? throw new NullMappingException<PostConfiguration, Post>(nameof(Id)),
                AuthorUserId = Author?.Id ?? throw new NullMappingException<PostConfiguration, Post>(nameof(Author)),
                Text = Text ?? throw new NullMappingException<PostConfiguration, Post>(nameof(Text)),
                Created = Created ?? throw new NullMappingException<PostConfiguration, Post>(nameof(Created))
            };
        }

        public PostInfo ToPostInfo()
        {
            return new PostInfo
            {
                Id = Id ?? throw new NullMappingException<PostConfiguration, Post>(nameof(Id)),
                Author = Author?.Name ?? throw new NullMappingException<PostConfiguration, Post>(nameof(Author)),
                Text = Text ?? throw new NullMappingException<PostConfiguration, Post>(nameof(Text)),
                Created = Created ?? throw new NullMappingException<PostConfiguration, Post>(nameof(Created))
            };
        }

        public PostData ToPostData(MappingBehavior mappingBehavior = MappingBehavior.Default)
        {
            return new PostData
            {
                Id = Id?.Value ?? throw new NullMappingException<PostConfiguration, PostData>(nameof(Id)),
                AuthorUserId = Author?.Id?.Value ?? throw new NullMappingException<PostConfiguration, PostData>(nameof(Author.Id)),
                Created = Created ?? throw new NullMappingException<PostConfiguration, PostData>(nameof(Created)),
                Content = new PostContentData
                {
                    PostId = Id?.Value ?? throw new NullMappingException<PostConfiguration, PostData>(nameof(PostData.Id)),
                    Text = Text ?? throw new NullMappingException<PostConfiguration, PostData>(nameof(PostData.Id)),
                },
                User = mappingBehavior.HasFlag(MappingBehavior.IncludeUser) ?
                    Author.ToUserData() : null
            };
        }

        [Flags]
        public enum MappingBehavior
        {
            Default = 0,
            IncludeUser = 1
        }
    }
}
