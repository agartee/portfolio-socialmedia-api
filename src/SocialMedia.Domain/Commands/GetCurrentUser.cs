using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("get user profile", HelpText = "Get an existing user's profile.")]
    public record GetCurrentUser : IRequest<User>
    {
    }

    public class GetCurrentUserHandler : IRequestHandler<GetCurrentUser, User>
    {
        private readonly IUserRepository userRepository;
        private readonly IUserContext userContext;

        public GetCurrentUserHandler(IUserRepository userRepository, IUserContext userContext)
        {
            this.userRepository = userRepository;
            this.userContext = userContext;
        }

        public async Task<User> Handle(GetCurrentUser request, CancellationToken cancellationToken)
        {
            return await userRepository.GetUser(userContext.UserId, cancellationToken);
        }
    }
}
