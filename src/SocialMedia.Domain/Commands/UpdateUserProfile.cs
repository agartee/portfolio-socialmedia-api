using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    public record UpdateUserProfile : IRequest<UserProfile>
    {
        public required string UserId { get; init; }
        public required string Name { get; init; }
        public required string Email { get; init; }
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
