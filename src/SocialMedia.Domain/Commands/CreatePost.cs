using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("create post", HelpText = "Create a new post.")]
    public record CreatePost : IRequest<PostInfo>
    {
        [Option(Required = false, HelpText = "Text content of the post")]
        public required string Text { get; init; }
    }

    public class CreatePostHandler : IRequestHandler<CreatePost, PostInfo>
    {
        private readonly IPostRepository postRepository;
        private readonly IUserContext userContext;

        public CreatePostHandler(IPostRepository postRepository, IUserContext userContext)
        {
            this.postRepository = postRepository;
            this.userContext = userContext;
        }

        public async Task<PostInfo> Handle(CreatePost request, CancellationToken cancellationToken)
        {
            var post = new Post
            {
                Id = PostId.NewId(),
                AuthorUserId = userContext.UserId,
                Text = request.Text,
                Created = DateTime.UtcNow
            };

            return await postRepository.CreatePost(post, cancellationToken);
        }
    }
}
