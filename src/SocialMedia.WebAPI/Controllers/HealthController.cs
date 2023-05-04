using Microsoft.AspNetCore.Mvc;

namespace SocialMedia.WebAPI.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [Route("/health")]
        public IActionResult Index()
        {
            return Ok("Up!");
        }
    }
}
