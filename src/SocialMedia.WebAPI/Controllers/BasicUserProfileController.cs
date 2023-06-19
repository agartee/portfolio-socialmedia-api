using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Domain.Commands;
using SocialMedia.WebAPI.Binders;

namespace SocialMedia.WebAPI.Controllers
{
    [ApiController]
    public class BasicUserProfileController : ControllerBase
    {
        private IMediator mediator;

        public BasicUserProfileController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        [Route("/basic-user-profile")]
        public async Task<IActionResult> Get([ModelBinder(typeof(UserIdFromClaimModelBinder))] GetBasicUserProfile request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpPatch]
        [Authorize]
        [Route("/basic-user-profile")]
        public async Task<IActionResult> Update([ModelBinder(typeof(UserIdFromClaimModelBinder))] UpdateBasicUserProfile request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
