using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Domain.Commands;
using System.Security.Claims;

namespace SocialMedia.WebAPI.Controllers
{
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private IMediator mediator;

        public UserProfileController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        [Route("/user-profile")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var idClaim = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
            var result = await mediator.Send(new GetUserProfile { Id = idClaim.Value }, cancellationToken);

            return Ok(result);
        }

        [HttpPatch]
        [Authorize]
        [Route("/user-profile")]
        public async Task<IActionResult> Patch(UpdateUserProfile request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
