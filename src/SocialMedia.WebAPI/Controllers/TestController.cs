using MediatR;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;

namespace SocialMedia.WebAPI.Controllers
{
    public class TestController : ControllerBase
    {
        private IMediator mediator;

        public TestController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Placeholder description for <see cref="Test"/>
        /// </summary>
        /// <remarks>
        /// Placeholder remarks for <see cref="Test"/>
        /// </remarks>
        /// <param name="request">request from query</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/test")]
        public async Task<IActionResult> Test([FromQuery] TestCommand request, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Placeholder description for <see cref = "Test2" />.This workflow is not supported by Swagger since the
        /// ParameterInfo cannot be mapped to the Swagger OpenApiParameter in an Operation Filter.
        /// </summary>
        /// <param name="id">id from query</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/test2")]
        public async Task<IActionResult> Test2([FromQuery] TestId id, CancellationToken cancellationToken)
        {
            return Ok(id);
        }

        /// <summary>
        /// Placeholder description for <see cref="Test3"/>
        /// </summary>
        /// <remarks>
        /// Placeholder remarks for <see cref="Test3"/>
        /// </remarks>
        /// <param name="id">id from route/path</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/test3/{id}")]
        public async Task<IActionResult> Test3([FromRoute] TestId id, CancellationToken cancellationToken)
        {
            return Ok(id);
        }

        /// <summary>
        /// Placeholder description for <see cref="Test4"/>
        /// </summary>
        /// <remarks>
        /// Placeholder remarks for <see cref="Test4"/>
        /// </remarks>
        /// <param name="id">id from route/path</param>
        /// <param name="request">request from query</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/test4/{id}")]
        public async Task<IActionResult> Test4([FromRoute] TestId id, [FromQuery] TestCommand request, CancellationToken cancellationToken)
        {
            return Ok(id);
        }

        /// <summary>
        /// Placeholder description for <see cref="Test5"/>
        /// </summary>
        /// <remarks>
        /// Placeholder remarks for <see cref="Test5"/>
        /// </remarks>
        /// <param name="id">id from route (not ID model)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/test5/{id}")]
        public async Task<IActionResult> Test5([FromRoute] int id, CancellationToken cancellationToken)
        {
            return Ok(id);
        }
    }
}
