using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Commands
{
    public class GetBasicUserProfileTests
    {
        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsUserProfile()
        {
            var userProfile = new BasicUserProfile
            {
                UserId = "id",
                Name = "name",
                Nickname = "nickname",
                Email = "me@here.com"
            };

            var repository = new Mock<IBasicUserProfileRepository>();
            repository.Setup(r => r.GetBasicUserProfile(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var handler = new GetBasicUserProfileHandler(repository.Object);

            var request = new GetBasicUserProfile
            {
                UserId = userProfile.UserId
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Be(userProfile);

            repository.Verify(m => m.GetBasicUserProfile(
                It.Is<string>(s => s == request.UserId),
                It.IsAny<CancellationToken>()));
        }
    }
}
