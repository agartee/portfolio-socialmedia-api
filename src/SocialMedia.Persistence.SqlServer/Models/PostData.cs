using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia.Persistence.SqlServer.Models
{
    [Table(TABLE_NAME)]
    public class PostData
    {
        public const string TABLE_NAME = "Post";

        public required Guid Id { get; set; }

        [MaxLength(100)]
        public required string UserId { get; set; }
        public required DateTime Created { get; set; }
    }
}
