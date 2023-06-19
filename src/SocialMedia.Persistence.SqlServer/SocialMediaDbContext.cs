using Microsoft.EntityFrameworkCore;
using SocialMedia.Persistence.SqlServer.Models;

namespace SocialMedia.Persistence.SqlServer
{
    public class SocialMediaDbContext : DbContext
    {
        public SocialMediaDbContext(DbContextOptions<SocialMediaDbContext> options) : base(options) { }

        public DbSet<PostData> Posts { get; set; }
        public DbSet<PostContentData> PostContents { get; set; }
        public DbSet<UserProfileData> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("SocialMedia");

            modelBuilder.Entity<PostData>()
                .HasOne(p => p.Content)
                .WithOne(c => c.Post);

            modelBuilder.Entity<PostData>()
                .HasOne(p => p.UserProfile)
                .WithMany(c => c.Posts);
        }
    }
}
