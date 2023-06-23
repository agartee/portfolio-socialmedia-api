using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;

namespace SocialMedia.Domain.Tests.Commands
{
    public class GetUserTests
    {
        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsUser()
        {
            var user = new User
            {
                UserId = "id",
                Name = "name",
                Email = "me@here.com"
            };

            var repository = new Mock<IUserRepository>();
            repository.Setup(r => r.GetUser(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var handler = new GetUserHandler(repository.Object);

            var request = new GetUser
            {
                UserId = user.UserId
            };

            var result = await handler.Handle(request, CancellationToken.None);

            result.Should().Be(user);

            repository.Verify(m => m.GetUser(
                It.Is<string>(s => s == request.UserId),
                It.IsAny<CancellationToken>()));
        }
    }
}
