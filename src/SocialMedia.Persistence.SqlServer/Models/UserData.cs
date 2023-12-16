using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Persistence.SqlServer.Models
{
    [Table(TABLE_NAME)]
    public class UserData
    {
        public const string TABLE_NAME = "User";

        [Key, MaxLength(100)]
        public required string Id { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        public required DateTime Created { get; set; }
        public required DateTime LastUpdated { get; set; }

        public List<PostData> Posts { get; set; } = new List<PostData>();
    }
}
