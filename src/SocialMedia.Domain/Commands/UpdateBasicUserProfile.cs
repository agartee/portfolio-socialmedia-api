using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("update user profile", HelpText = "Update an existing user's profile.")]
    public record UpdateBasicUserProfile : IRequest<BasicUserProfile>
    {
        [Option(Required = false, HelpText = "User's ID")]
        public required string UserId { get; init; }

        [Option(Required = false, HelpText = "User's name")]
        public string? Name { get; init; }

        [Option(Required = false, HelpText = "User's nickname")]
        public string? Nickname { get; init; }

        [Option(Required = false, HelpText = "User's email address")]
        public string? Email { get; init; }
    }

    public class UpdateBasicUserProfileHandler : IRequestHandler<UpdateBasicUserProfile, BasicUserProfile>
    {
        private readonly IBasicUserProfileRepository basicUserProfileRepository;

        public UpdateBasicUserProfileHandler(IBasicUserProfileRepository userProfileRepository)
        {
            this.basicUserProfileRepository = userProfileRepository;
        }

        public async Task<BasicUserProfile> Handle(UpdateBasicUserProfile request, CancellationToken cancellationToken)
        {
            var userProfile = new BasicUserProfile
            {
                UserId = request.UserId,
                Name = request.Name,
                Nickname = request.Nickname,
                Email = request.Email
            };

            return await basicUserProfileRepository.UpdateBasicUserProfile(userProfile, cancellationToken);
        }
    }
}
