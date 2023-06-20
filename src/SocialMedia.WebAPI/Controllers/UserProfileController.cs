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
        public async Task<IActionResult> Get([ModelBinder(typeof(UserIdFromClaimModelBinder))] GetUserProfile request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpPatch]
        [Authorize]
        [Route("/user-profile")]
        public async Task<IActionResult> Update([ModelBinder(typeof(UserIdFromClaimModelBinder))] UpdateUserProfile request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [Route("/user-profile")]
        public async Task<IActionResult> Synchronize([ModelBinder(typeof(UserIdFromClaimModelBinder))] SynchronizeUserProfile request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
