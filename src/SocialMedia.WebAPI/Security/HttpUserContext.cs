using SocialMedia.Domain.Models;
using SocialMedia.Domain.Services;
using System.Security.Claims;

namespace SocialMedia.WebAPI.Security
{
    public class HttpUserContext : IUserContext
    {
        public HttpUserContext(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext == null)
                throw new InvalidOperationException($"{nameof(IHttpContextAccessor.HttpContext)} cannot be null.");

            User = httpContextAccessor.HttpContext.User;
            UserId = GetUserIdFromClaim(User);
        }

        private static UserId GetUserIdFromClaim(ClaimsPrincipal user)
        {
            var idClaim = user.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            return idClaim != null
                ? new UserId(idClaim.Value)
                : throw new InvalidOperationException($"Unable to determine user name.");
        }

        public UserId UserId { get; init; }
        public ClaimsPrincipal User { get; init; }
    }
}
