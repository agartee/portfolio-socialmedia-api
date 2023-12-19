using MediatR;
using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Commands
{
    public record TestCommand : IRequest<string>
    {
        /// <summary>
        /// Placeholder description for SomeId
        /// </summary>
        public required TestId SomeId { get; init; }
        /// <summary>
        /// Placeholder description for Something
        /// </summary>
        public required string Something { get; init; }
    }

    public class TestCommandHandler : IRequestHandler<TestCommand, string>
    {
        public Task<string> Handle(TestCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult("ok!");
        }
    }
}
