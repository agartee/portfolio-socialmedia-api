using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("get user profile", HelpText = "Get an existing user's profile.")]
    public record GetUser : IRequest<User>
    {
        [Option(Required = false, HelpText = "User's ID")]
        public required UserId UserId { get; init; }
    }

    public class GetUserHandler : IRequestHandler<GetUser, User>
    {
        private readonly IUserRepository userRepository;

        public GetUserHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<User> Handle(GetUser request, CancellationToken cancellationToken)
        {
            return await userRepository.GetUser(request.UserId, cancellationToken);
        }
    }
}
