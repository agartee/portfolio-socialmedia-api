using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    public record UpdateUser : IRequest<User>
    {
        public required string UserId { get; init; }
        public required string Name { get; init; }
        public required string Email { get; init; }
    }

    public class UpdateUserHandler : IRequestHandler<UpdateUser, User>
    {
        private readonly IUserRepository userRepository;

        public UpdateUserHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<User> Handle(UpdateUser request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                UserId = request.UserId,
                Name = request.Name,
                Email = request.Email
            };

            return await userRepository.UpdateUser(user, cancellationToken);
        }
    }
}
