using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("create post", HelpText = "Create a new post.")]
    public class CreatePost : IRequest<PostInfo>
    {
        [Option(Required = false, HelpText = "User's ID")]
        public required string UserId { get; init; }

        [Option(Required = false, HelpText = "Text content of the post")]
        public required string Text { get; init; }
    }

    public class CreatePostHandler : IRequestHandler<CreatePost, PostInfo>
    {
        private readonly IPostRepository postRepository;

        public CreatePostHandler(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public async Task<PostInfo> Handle(CreatePost request, CancellationToken cancellationToken)
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Text = request.Text,
                Created = DateTime.UtcNow
            };

            return await postRepository.CreatePost(post, cancellationToken);
        }
    }
}
