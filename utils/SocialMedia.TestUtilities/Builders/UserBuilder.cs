using SocialMedia.Domain.Models;
using SocialMedia.TestUtilities.Models;

namespace SocialMedia.TestUtilities.Builders
{
    public class UserBuilder
    {
        private readonly DataRandomizer<UserData> users = DataRandomizer.Create(
            new UserData { Id = new UserId("idp|1"), Name = "Monica" },
            new UserData { Id = new UserId("idp|2"), Name = "Rachael" },
            new UserData { Id = new UserId("idp|3"), Name = "Phoebe" },
            new UserData { Id = new UserId("idp|4"), Name = "Chandler" },
            new UserData { Id = new UserId("idp|5"), Name = "Ross" },
            new UserData { Id = new UserId("idp|6"), Name = "Joey" });

        public UserConfiguration CreateUser()
        {
            var nextUser = users.Next();

            return new UserConfiguration()
                .WithId(nextUser.Id)
                .WithName(nextUser.Name);
        }

        private record UserData
        {
            public required UserId Id { get; init; }
            public required string Name { get; init; }
        }
    }
}
