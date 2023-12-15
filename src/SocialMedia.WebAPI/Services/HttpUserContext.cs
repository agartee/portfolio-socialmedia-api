using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using System.Security.Claims;

namespace SocialMedia.WebAPI.Services
{
    public class HttpUserContext : IUserContext
    {
        public HttpUserContext(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext == null)
                throw new InvalidOperationException($"{nameof(httpContextAccessor.HttpContext)} cannot be null.");

            User = httpContextAccessor.HttpContext.User;
            UserId = GetUserIdFromClaim(User);
        }

        private static UserId GetUserIdFromClaim(ClaimsPrincipal user)
        {
            var idClaim = user.Claims
                .First(c => c.Type == ClaimTypes.NameIdentifier);

            return idClaim != null
                ? new UserId(idClaim.Value)
                : throw new InvalidOperationException($"Unable to determine user name.");
        }

        public UserId? UserId { get; init; }
        public ClaimsPrincipal User { get; init; }

    }
}
