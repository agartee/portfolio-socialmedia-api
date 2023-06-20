using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Persistence.SqlServer.Models
{
    [Table(TABLE_NAME)]
    public class UserProfileData
    {
        public const string TABLE_NAME = "UserProfile";

        [Key, MaxLength(100)]
        public required string UserId { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(255)]
        public required string Email { get; set; }

        public required DateTime Created { get; set; }
        public required DateTime LastUpdated { get; set; }

        public List<PostData> Posts { get; set; } = new List<PostData>();
    }
}
