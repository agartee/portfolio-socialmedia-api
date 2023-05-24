using MediatR;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Domain.Commands;
using SocialMedia.WebAPI.Binders;

namespace SocialMedia.WebAPI.Controllers
{
    public class PostController : ControllerBase
    {
        private IMediator mediator;

        public PostController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<IActionResult> Put([ModelBinder(typeof(IdFromClaimModelBinder))] CreatePost request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
