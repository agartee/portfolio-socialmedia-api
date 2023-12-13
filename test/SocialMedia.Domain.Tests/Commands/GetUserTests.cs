using FluentAssertions;
using Moq;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using SocialMedia.TestUtilities.Builders;

namespace SocialMedia.Domain.Tests.Commands
{
    public class GetUserTests
    {
        private readonly GetUserHandler handler;
        private readonly Mock<IUserRepository> userRepository;
        private readonly UserBuilder userBuilder = new();

        public GetUserTests()
        {
            userRepository = new Mock<IUserRepository>();
            handler = new GetUserHandler(userRepository.Object);
        }

        [Fact]
        public async Task Handle_CallsRepositoryAndReturnsUser()
        {
            var user = userBuilder.CreateUser().ToUser();

            userRepository.Setup(r => r.GetUser(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var request = new GetUser { UserId = user.Id };
            var cancellationToken = CancellationToken.None;

            var result = await handler.Handle(request, cancellationToken);

            result.Should().Be(user);

            userRepository.Verify(m => m.GetUser(
                It.Is<UserId>(s => s == request.UserId!),
                cancellationToken));
        }
    }
}
