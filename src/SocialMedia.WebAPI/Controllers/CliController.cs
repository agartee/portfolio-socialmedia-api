using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Domain.Exceptions;
using SocialMedia.Domain.Services;

namespace SocialMedia.WebAPI.Controllers
{
    [ApiController]
    public class CliController : ControllerBase
    {
        private readonly ICliRequestBuilder requestBuilder;
        private readonly IMediator mediator;

        public CliController(ICliRequestBuilder requestBuilder, IMediator mediator)
        {
            this.requestBuilder = requestBuilder;
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("/cli")]
        [Authorize(policy: "admin")]
        public async Task<IActionResult> Run([FromBody] string commandText, CancellationToken cancellationToken)
        {
            try
            {
                var request = requestBuilder.BuildRequest(commandText);
                var response = await mediator.Send(request, cancellationToken);

                return Ok(response);
            }
            catch (CommandLineParsingException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
