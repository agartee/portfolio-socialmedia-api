using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Domain.Commands;
using SocialMedia.WebAPI.Binders;

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
        public async Task<IActionResult> Get([ModelBinder(typeof(IdFromClaimModelBinder))] GetUserProfile request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpPatch]
        [Authorize]
        [Route("/user-profile")]
        public async Task<IActionResult> Patch([ModelBinder(typeof(IdFromClaimModelBinder))] UpdateUserProfile request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
