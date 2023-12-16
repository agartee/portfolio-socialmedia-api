using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Domain.Commands;

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
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetCurrentUser(), cancellationToken);

            return Ok(result);
        }

        [HttpPatch]
        [Authorize]
        [Route("/user-profile")]
        public async Task<IActionResult> Update(UpdateCurrentUser request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [Route("/user-profile")]
        public async Task<IActionResult> Synchronize(SynchronizeCurrentUser request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
