using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("get user profile", HelpText = "Get an existing user's profile.")]
    public record GetUserProfile : IRequest<UserProfile>
    {
        [Option(Required = false, HelpText = "User's ID")]
        public required string UserId { get; init; }
    }

    public class GetUserProfileHandler : IRequestHandler<GetUserProfile, UserProfile>
    {
        private readonly IUserProfileRepository userProfileRepository;

        public GetUserProfileHandler(IUserProfileRepository userProfileRepository)
        {
            this.userProfileRepository = userProfileRepository;
        }

        public async Task<UserProfile> Handle(GetUserProfile request, CancellationToken cancellationToken)
        {
            return await userProfileRepository.GetUserProfile(request.UserId, cancellationToken);
        }
    }
}
