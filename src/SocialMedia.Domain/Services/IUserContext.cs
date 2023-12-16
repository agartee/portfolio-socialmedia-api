using SocialMedia.Domain.Models;
using System.Security.Claims;

namespace SocialMedia.Domain.Services
{
    public interface IUserContext
    {
        public UserId UserId { get; }
        public ClaimsPrincipal User { get; }
    }
}
