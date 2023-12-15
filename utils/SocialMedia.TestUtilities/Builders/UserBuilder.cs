using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Models;
using SocialMedia.TestUtilities.Exceptions;

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

    public record UserConfiguration
    {
        public UserId? Id { get; private set; }
        public string? Name { get; private set; }

        public UserConfiguration WithId(UserId? id)
        {
            Id = id;
            return this;
        }

        public UserConfiguration WithName(string? author)
        {
            Name = author;
            return this;
        }

        public User ToUser()
        {
            return new User
            {
                Id = Id ?? throw new NullMappingException<UserConfiguration, User>(nameof(Id)),
                Name = Name ?? throw new NullMappingException<UserConfiguration, User>(nameof(Name)),
            };
        }

        public UserData ToUserData()
        {
            return new UserData
            {
                Id = Id?.Value ?? throw new NullMappingException<UserConfiguration, UserData>(nameof(UserData.Id)),
                Name = Name ?? throw new NullMappingException<UserConfiguration, UserData>(nameof(UserData.Name)),
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
        }
    }
}
