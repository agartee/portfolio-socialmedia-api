using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Domain.Commands;
using SocialMedia.WebAPI.Binders;

namespace SocialMedia.WebAPI.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        [Route("/user-profile")]
        public async Task<IActionResult> Get([ModelBinder(typeof(UserIdFromClaimModelBinder))] GetUser request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpPatch]
        [Authorize]
        [Route("/user-profile")]
        public async Task<IActionResult> Update([ModelBinder(typeof(UserIdFromClaimModelBinder))] UpdateUser request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [Route("/user-profile")]
        public async Task<IActionResult> Synchronize([ModelBinder(typeof(UserIdFromClaimModelBinder))] SynchronizeUser request,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
