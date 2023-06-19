using CommandLine;
using MediatR;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Commands
{
    [Verb("get extended user profile", HelpText = "Get an existing user's extended user profile.")]
    public record GetExtendedUserProfile : IRequest<ExtendedUserProfile>
    {
        [Option(Required = false, HelpText = "User's ID")]
        public required string UserId { get; init; }
    }

    public class GetExtendedUserProfileHandler : IRequestHandler<GetExtendedUserProfile, ExtendedUserProfile>
    {
        private readonly IExtendedUserProfileRepository extendedUserProfileRepository;
        public GetExtendedUserProfileHandler(IExtendedUserProfileRepository extendedUserProfileRepository)
        {
            this.extendedUserProfileRepository = extendedUserProfileRepository;
        }

        public async Task<ExtendedUserProfile> Handle(GetExtendedUserProfile request, CancellationToken cancellationToken)
        {
            return await extendedUserProfileRepository.GetExtendedUserProfile(request.UserId, cancellationToken);
        }
    }
}
