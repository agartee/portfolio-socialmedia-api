using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("update user profile", HelpText = "Update an existing user's profile.")]
    public record UpdateUserProfile : IRequest<UserProfile>
    {
        [Option(Required = false, HelpText = "User's ID")]
        public required string UserId { get; init; }

        [Option(Required = false, HelpText = "User's name")]
        public string? Name { get; init; }

        [Option(Required = false, HelpText = "User's email address")]
        public string? Email { get; init; }
    }

    public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfile, UserProfile>
    {
        private readonly IUserProfileRepository userProfileRepository;

        public UpdateUserProfileHandler(IUserProfileRepository userProfileRepository)
        {
            this.userProfileRepository = userProfileRepository;
        }

        public async Task<UserProfile> Handle(UpdateUserProfile request, CancellationToken cancellationToken)
        {
            var userProfile = new UserProfile
            {
                UserId = request.UserId,
                Name = request.Name,
                Email = request.Email
            };

            return await userProfileRepository.UpdateUserProfile(userProfile, cancellationToken);
        }
    }
}
