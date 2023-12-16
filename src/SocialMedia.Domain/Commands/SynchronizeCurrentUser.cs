using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    public record SynchronizeCurrentUser : IRequest<User>
    {
        public required string Name { get; init; }
    }

    public class SynchronizeCurrentUserHandler : IRequestHandler<SynchronizeCurrentUser, User>
    {
        private readonly IUserSynchronizer userSynchronizer;
        private readonly IUserContext userContext;

        public SynchronizeCurrentUserHandler(IUserSynchronizer userSynchronizer, IUserContext userContext)
        {
            this.userSynchronizer = userSynchronizer;
            this.userContext = userContext;
        }

        public async Task<User> Handle(SynchronizeCurrentUser request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = userContext.UserId,
                Name = request.Name
            };

            return await userSynchronizer.SyncUser(user, cancellationToken);
        }
    }
}
