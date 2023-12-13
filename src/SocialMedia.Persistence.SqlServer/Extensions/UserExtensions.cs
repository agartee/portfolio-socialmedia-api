using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Models;

namespace SocialMedia.Persistence.SqlServer.Extensions
{
    public static class UserExtensions
    {
        public static User ToUser(this UserData user)
        {
            return new User
            {
                Id = new UserId(user.Id),
                Name = user.Name
            };
        }
    }
}
