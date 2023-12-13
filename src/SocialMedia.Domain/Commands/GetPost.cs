using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("get post", HelpText = "Get an existing post.")]
    public record GetPost : IRequest<PostInfo>
    {
        [Option(Required = false, HelpText = "Post's ID")]
        public required PostId Id { get; init; }
    }

    public class GetPostHandler : IRequestHandler<GetPost, PostInfo>
    {
        private readonly IPostRepository postRepository;

        public GetPostHandler(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public Task<PostInfo> Handle(GetPost request, CancellationToken cancellationToken)
        {
            return postRepository.DemandPost(request.Id, cancellationToken);
        }
    }
}
