using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    public record UpdateCurrentUser : IRequest<User>
    {
        public required string Name { get; init; }
    }

    public class UpdateCurrentUserHandler : IRequestHandler<UpdateCurrentUser, User>
    {
        private readonly IUserRepository userRepository;
        private readonly IUserContext userContext;

        public UpdateCurrentUserHandler(IUserRepository userRepository, IUserContext userContext)
        {
            this.userRepository = userRepository;
            this.userContext = userContext;
        }

        public async Task<User> Handle(UpdateCurrentUser request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = userContext.UserId,
                Name = request.Name
            };

            return await userRepository.UpdateUser(user, cancellationToken);
        }
    }
}
