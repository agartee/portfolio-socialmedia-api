using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("update user profile", HelpText = "Update an existing user's profile.")]
    public record UpdateExtendedUserProfile : IRequest<ExtendedUserProfile>
    {
        [Option(Required = false, HelpText = "User's ID")]
        public required string UserId { get; init; }

        [Option(Required = false, HelpText = "User's display name")]
        public string? DisplayName { get; init; }
    }

    public class UpdateExtendedUserProfileHandler : IRequestHandler<UpdateExtendedUserProfile, ExtendedUserProfile>
    {
        private readonly IExtendedUserProfileRepository extendedUserProfileRepository;

        public UpdateExtendedUserProfileHandler(IExtendedUserProfileRepository userProfileRepository)
        {
            this.extendedUserProfileRepository = userProfileRepository;
        }

        public async Task<ExtendedUserProfile> Handle(UpdateExtendedUserProfile request, CancellationToken cancellationToken)
        {
            var userProfile = new ExtendedUserProfile
            {
                UserId = request.UserId,
                DisplayName = request.DisplayName
            };

            return await extendedUserProfileRepository.UpdateExtendedUserProfile(userProfile, cancellationToken);
        }
    }
}
