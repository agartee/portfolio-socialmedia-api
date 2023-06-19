using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Commands
{
    public class GetExtendedUserProfileTests
    {
        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsUserProfile()
        {
            var userProfile = new ExtendedUserProfile
            {
                UserId = "id",
                DisplayName = "display name"
            };

            var repository = new Mock<IExtendedUserProfileRepository>();
            repository.Setup(r => r.GetExtendedUserProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var handler = new GetExtendedUserProfileHandler(repository.Object);

            var request = new GetExtendedUserProfile
            {
                UserId = userProfile.UserId
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Be(userProfile);

            repository.Verify(m => m.GetExtendedUserProfile(
                It.Is<string>(s => s == request.UserId),
                It.IsAny<CancellationToken>()));
        }
    }
}
