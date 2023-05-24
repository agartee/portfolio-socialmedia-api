using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("create post", HelpText = "Create a new post.")]
    public class CreatePost : IRequest<Post>
    {
        [Option(Required = false, HelpText = "User's ID")]
        public required string UserId { get; init; }

        [Option(Required = false, HelpText = "Text content of the post")]
        public required string Text { get; init; }
    }

    public class CreatePostHandler : IRequestHandler<CreatePost, Post>
    {
        private readonly IPostRepository postRepository;

        public CreatePostHandler(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public async Task<Post> Handle(CreatePost request, CancellationToken cancellationToken)
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Text = request.Text,
                Created = DateTime.UtcNow
            };

            await postRepository.CreatePost(post, cancellationToken);

            return post;
        }
    }
}
