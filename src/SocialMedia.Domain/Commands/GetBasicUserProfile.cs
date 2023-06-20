using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("get basic user profile", HelpText = "Get an existing user's basic user profile.")]
    public record GetBasicUserProfile : IRequest<BasicUserProfile>
    {
        [Option(Required = false, HelpText = "User's ID")]
        public required string UserId { get; init; }
    }

    public class GetBasicUserProfileHandler : IRequestHandler<GetBasicUserProfile, BasicUserProfile>
    {
        private readonly IBasicUserProfileRepository basicUserProfileRepository;

        public GetBasicUserProfileHandler(IBasicUserProfileRepository userProfileRepository)
        {
            this.basicUserProfileRepository = userProfileRepository;
        }

        public async Task<BasicUserProfile> Handle(GetBasicUserProfile request, CancellationToken cancellationToken)
        {
            return await basicUserProfileRepository.GetBasicUserProfile(request.UserId, cancellationToken);
        }
    }
}
