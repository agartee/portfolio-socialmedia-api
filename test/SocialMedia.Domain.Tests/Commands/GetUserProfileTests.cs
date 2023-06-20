using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Commands
{
    public class GetUserProfileTests
    {
        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsUserProfile()
        {
            var userProfile = new UserProfile
            {
                UserId = "id",
                Name = "name",
                Email = "me@here.com"
            };

            var repository = new Mock<IUserProfileRepository>();
            repository.Setup(r => r.GetUserProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var handler = new GetUserProfileHandler(repository.Object);

            var request = new GetUserProfile
            {
                UserId = userProfile.UserId
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Be(userProfile);

            repository.Verify(m => m.GetUserProfile(
                It.Is<string>(s => s == request.UserId),
                It.IsAny<CancellationToken>()));
        }
    }
}
