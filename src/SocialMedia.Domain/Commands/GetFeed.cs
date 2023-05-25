using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("get feed", HelpText = "Get the latest feed.")]
    public class GetFeed : IRequest<IEnumerable<Post>>
    {
    }

    public class GetFeedHandler : IRequestHandler<GetFeed, IEnumerable<Post>>
    {
        private readonly IPostRepository postRepository;

        public GetFeedHandler(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public async Task<IEnumerable<Post>> Handle(GetFeed request, CancellationToken cancellationToken)
        {
            return await postRepository.GetAllPosts(cancellationToken);
        }
    }
}
