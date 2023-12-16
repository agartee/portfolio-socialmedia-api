using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.WebAPI.Controllers;

namespace SocialMedia.WebAPI.Tests.Controllers
{
    public class HealthControllerTests
    {
        private readonly HealthController controller = new();

        [Fact]
        public void Get_ReturnsUp()
        {
            var result = controller.Index();

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be("Up!");
        }
    }
}
