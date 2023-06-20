using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Domain.Commands;
using SocialMedia.WebAPI.Binders;

namespace SocialMedia.WebAPI.Controllers
{
    [ApiController]
    public class ExtendedUserProfileController : ControllerBase
    {
        private IMediator mediator;

        public ExtendedUserProfileController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        [Route("/extended-user-profile")]
        public async Task<IActionResult> Get([ModelBinder(typeof(UserIdFromClaimModelBinder))] GetExtendedUserProfile request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpPatch]
        [Authorize]
        [Route("/extended-user-profile")]
        public async Task<IActionResult> Update([ModelBinder(typeof(UserIdFromClaimModelBinder))] UpdateExtendedUserProfile request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
