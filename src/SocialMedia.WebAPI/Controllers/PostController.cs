using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;

namespace SocialMedia.WebAPI.Controllers
{
    public class PostController : ControllerBase
    {
        private IMediator mediator;

        public PostController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPut]
        [Authorize]
        [Route("/post")]
        public async Task<IActionResult> Create([FromBody] CreatePost request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [Route("/post/{id}")]
        public async Task<IActionResult> Create([FromRoute] PostId postId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetPost { Id = postId }, cancellationToken);

            return Ok(result);
        }
    }
}
