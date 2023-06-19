using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Commands
{
    public class UpdateBasicUserProfileTests
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
            repository.Setup(r => r.UpdateBasicUserProfile(It.IsAny<BasicUserProfile>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userProfile);

            var handler = new UpdateBasicUserProfileHandler(repository.Object);

            var request = new UpdateBasicUserProfile
            {
                UserId = userProfile.UserId,
                Name = userProfile.Name,
                Nickname = userProfile.Nickname,
                Email = userProfile.Email
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Be(userProfile);

            repository.Verify(m => m.UpdateBasicUserProfile(
                It.Is<BasicUserProfile>(s => s == userProfile),
                It.IsAny<CancellationToken>()));
        }
    }
}
