using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Models;
using SocialMedia.TestUtilities.Exceptions;

namespace SocialMedia.TestUtilities.Models
{
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
                Id = Id ?? throw new NullMappingException<Post>(nameof(Post.Id)),
                AuthorUserId = Author?.Id ?? throw new NullMappingException<Post>(nameof(Post.AuthorUserId)),
                Text = Text ?? throw new NullMappingException<Post>(nameof(Post.Text)),
                Created = Created ?? throw new NullMappingException<Post>(nameof(Post.Created))
            };
        }

        public PostInfo ToPostInfo()
        {
            return new PostInfo
            {
                Id = Id ?? throw new NullMappingException<PostInfo>(nameof(PostInfo.Id)),
                Author = Author?.Name ?? throw new NullMappingException<PostInfo>(nameof(PostInfo.Author)),
                Text = Text ?? throw new NullMappingException<PostInfo>(nameof(PostInfo.Text)),
                Created = Created ?? throw new NullMappingException<PostInfo>(nameof(PostInfo.Created))
            };
        }

        public PostData ToPostData() => ToPostData(MappingContextScope.Current);

        public PostData ToPostData(MappingContext context)
        {
            context.SetState(this, MappingState.Added);

            return new PostData
            {
                Id = Id?.Value ?? throw new NullMappingException<PostData>(nameof(PostData.Id)),
                AuthorUserId = Author?.Id?.Value ?? throw new NullMappingException<PostData>(nameof(PostData.AuthorUserId)),
                Created = Created ?? throw new NullMappingException<PostInfo>(nameof(PostData.Created)),
                Content = new PostContentData
                {
                    PostId = Id?.Value ?? throw new NullMappingException<PostInfo>(nameof(PostData.Id)),
                    Text = Text ?? throw new NullMappingException<PostInfo>(nameof(PostData.Id)),
                },
                User = context.GetState(Author) == MappingState.Detached ? Author.ToUserData(context) : null
            };
        }
    }
}
