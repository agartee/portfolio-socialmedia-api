using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SocialMedia.TestUtilities.Builders;
using SocialMedia.WebAPI.Security;
using System.Security.Claims;

namespace SocialMedia.WebAPI.Tests.Security
{
    public class HttpUserContextTests
    {
        private readonly Mock<IHttpContextAccessor> httpContextAccessor = new();
        private readonly UserBuilder userBuilder = new();

        [Fact]
        public void GetUser_ReturnsUserFromHttpContext()
        {
            var user = userBuilder.CreateUser().ToUser();
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.Value),
                }, "TestAuthentication"));

            httpContextAccessor.Setup(a => a.HttpContext)
                .Returns(new DefaultHttpContext { User = claimsPrincipal });

            var result = new HttpUserContext(httpContextAccessor.Object);

            result.UserId.Should().Be(user.Id);
            result.User.Should().Be(claimsPrincipal);
        }

        [Fact]
        public void Create_WhenHttpContextIsNull()
        {
            var action = () => new HttpUserContext(httpContextAccessor.Object);

            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"*{nameof(IHttpContextAccessor.HttpContext)}*");
        }

        [Fact]
        public void GetUser_WhenNameClaimNotExists_Throws()
        {
            var user = userBuilder.CreateUser().ToUser();
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(Enumerable.Empty<Claim>(),
                "TestAuthentication"));

            httpContextAccessor.Setup(a => a.HttpContext)
                .Returns(new DefaultHttpContext { User = claimsPrincipal });

            var action = () => new HttpUserContext(httpContextAccessor.Object);

            action.Should().Throw<InvalidOperationException>()
                .WithMessage($"Unable to determine user name.");
        }
    }
}
