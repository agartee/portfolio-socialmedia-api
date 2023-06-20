using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("get basic user profile", HelpText = "Get an existing user's basic user profile.")]
    public record GetUserProfile : IRequest<UserProfile>
    {
        [Option(Required = false, HelpText = "User's ID")]
        public required string UserId { get; init; }
    }

    public class GetUserProfileHandler : IRequestHandler<GetUserProfile, UserProfile>
    {
        private readonly IUserProfileRepository basicUserProfileRepository;

        public GetUserProfileHandler(IUserProfileRepository userProfileRepository)
        {
            this.basicUserProfileRepository = userProfileRepository;
        }

        public async Task<UserProfile> Handle(GetUserProfile request, CancellationToken cancellationToken)
        {
            return await basicUserProfileRepository.GetUserProfile(request.UserId, cancellationToken);
        }
    }
}
