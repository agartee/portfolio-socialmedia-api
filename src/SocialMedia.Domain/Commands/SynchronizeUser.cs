using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    public record SynchronizeUser : IRequest<User>
    {
        public required string UserId { get; init; }
        public required string Name { get; init; }
    }

    public class SynchronizeUserHandler : IRequestHandler<SynchronizeUser, User>
    {
        private readonly IUserSynchronizer userSynchronizer;

        public SynchronizeUserHandler(IUserSynchronizer userSynchronizer)
        {
            this.userSynchronizer = userSynchronizer;
        }

        public async Task<User> Handle(SynchronizeUser request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                UserId = request.UserId,
                Name = request.Name
            };

            return await userSynchronizer.SyncUser(user, cancellationToken);
        }
    }
}
