using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Domain.Commands;

namespace SocialMedia.WebAPI.Controllers
{
    public class FeedController : ControllerBase
    {
        private IMediator mediator;

        public FeedController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        [Route("/feed")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetFeed(), cancellationToken);

            return Ok(result);
        }
    }
}
