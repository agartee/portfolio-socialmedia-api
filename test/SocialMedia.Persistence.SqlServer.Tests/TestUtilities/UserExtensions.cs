using SocialMedia.Persistence.SqlServer.Models;
using SocialMedia.TestUtilities.Builders;
using SocialMedia.TestUtilities.Exceptions;

namespace SocialMedia.Persistence.SqlServer.Tests.TestUtilities
{
    internal static class UserExtensions
    {
        internal static UserData ToUserData(this UserConfiguration user)
        {
            return new UserData
            {
                Id = user.Id?.Value ?? throw new NullMappingException<UserConfiguration, UserData>(nameof(UserData.Id)),
                Name = user.Name ?? throw new NullMappingException<UserConfiguration, UserData>(nameof(UserData.Name)),
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
        }
    }
}
