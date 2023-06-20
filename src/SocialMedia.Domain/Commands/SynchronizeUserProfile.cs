using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    public record SynchronizeUserProfile : IRequest<UserProfile>
    {
        public required string UserId { get; init; }
        public required string Name { get; init; }
        public required string Email { get; init; }
    }

    public class SynchronizeUserProfileHandler : IRequestHandler<SynchronizeUserProfile, UserProfile>
    {
        private readonly IUserProfileSynchronizer userProfileSynchronizer;

        public SynchronizeUserProfileHandler(IUserProfileSynchronizer userProfileSynchronizer)
        {
            this.userProfileSynchronizer = userProfileSynchronizer;
        }

        public async Task<UserProfile> Handle(SynchronizeUserProfile request, CancellationToken cancellationToken)
        {
            var userProfile = new UserProfile
            {
                UserId = request.UserId,
                Name = request.Name,
                Email = request.Email
            };

            return await userProfileSynchronizer.UpdateUserProfile(userProfile, cancellationToken);
        }
    }
}
