using Microsoft.AspNetCore.Mvc;
using SocialMedia.Domain.Commands;
using SocialMedia.Domain.Models;

namespace SocialMedia.WebAPI.Endpoints
{
    public static class CreatePostMapping
    {
        public static IEndpointRouteBuilder MapCreatePost(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost(
                "post",
                (
                    CreatePost command,
                    CreatePostHandler handler,
                    CancellationToken cancellationToken) => handler.Handle(command, cancellationToken
                ))
                .WithName("CreatePost")
                .WithMetadata(new EndpointNameMetadata("CreatePost"))
                .WithMetadata(new HttpMethodMetadata(new[] { "POST" }))
                .WithMetadata(new ProducesResponseTypeAttribute(typeof(PostInfo), StatusCodes.Status200OK))
                .WithTags("Post")
                .WithDescription("Creates a new Post")
                .RequireAuthorization();

            return endpoints;
        }
    }
}
