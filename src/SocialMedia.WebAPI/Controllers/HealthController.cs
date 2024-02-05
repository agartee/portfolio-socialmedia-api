using Microsoft.AspNetCore.Mvc;
using SocialMedia.WebAPI.Configuration;

namespace SocialMedia.WebAPI.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly VersionInfo version;

        public HealthController(VersionInfo appInfo)
        {
            this.version = appInfo;
        }

        [HttpGet]
        [Route("/health")]
        public IActionResult Index()
        {
            return Ok(new
            {
                Availability = "Up",
                version.AssemblyVersion,
                version.ProductVersion
            });
        }
    }
}
