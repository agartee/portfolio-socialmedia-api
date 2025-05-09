using SocialMedia.Domain.Models;
using SocialMedia.Persistence.SqlServer.Models;
using SocialMedia.TestUtilities.Exceptions;

namespace SocialMedia.TestUtilities.Models
{
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
                Id = Id ?? throw new NullMappingException<User>(nameof(User.Id)),
                Name = Name ?? throw new NullMappingException<User>(nameof(User.Name)),
            };
        }

        public UserData ToUserData() => ToUserData(MappingContextScope.Current);

        public UserData ToUserData(MappingContext context)
        {
            context.SetState(this, MappingState.Added);

            return new UserData
            {
                Id = Id?.Value ?? throw new NullMappingException<UserData>(nameof(UserData.Id)),
                Name = Name ?? throw new NullMappingException<UserData>(nameof(UserData.Name)),
                Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
        }
    }
}
