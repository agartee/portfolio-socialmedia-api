using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.WebAPI.Configuration;
using SocialMedia.WebAPI.Controllers;

namespace SocialMedia.WebAPI.Tests.Controllers
{
    public class HealthControllerTests
    {
        private readonly HealthController controller;
        private readonly VersionInfo version = new VersionInfo
        {
            AssemblyVersion = "1.0.0",
            ProductVersion = "1.0.0-beta1"
        };

        public HealthControllerTests()
        {
            controller = new HealthController(version);
        }

        [Fact]
        public void Get_ReturnsUp()
        {
            var result = controller.Index();

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().BeEquivalentTo(new
            {
                Availability = "Up",
                version.AssemblyVersion,
                version.ProductVersion
            });
        }
    }
}
