using Microsoft.AspNetCore.Mvc;
using SocialMedia.WebAPI.Configuration;

namespace SocialMedia.WebAPI.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly VersionInfo appInfo;

        public HealthController(VersionInfo appInfo)
        {
            this.appInfo = appInfo;
        }

        [HttpGet]
        [Route("/health")]
        public IActionResult Index()
        {
            return Ok(new
            {
                Availability = "Up!",
                appInfo.Version,
                appInfo.Build
            });
        }
    }
}
