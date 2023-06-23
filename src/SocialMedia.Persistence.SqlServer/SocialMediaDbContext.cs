using Microsoft.EntityFrameworkCore;
using SocialMedia.Persistence.SqlServer.Models;

namespace SocialMedia.Persistence.SqlServer
{
    public class SocialMediaDbContext : DbContext
    {
        public const string SCHEMA_NAME = "SocialMedia";

        public SocialMediaDbContext(DbContextOptions<SocialMediaDbContext> options) : base(options) { }

        public DbSet<PostData> Posts { get; set; }
        public DbSet<PostContentData> PostContents { get; set; }
        public DbSet<UserData> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(SCHEMA_NAME);

            modelBuilder.Entity<PostData>()
                .HasOne(p => p.Content)
                .WithOne(c => c.Post);

            modelBuilder.Entity<PostData>()
                .HasOne(p => p.User)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.UserId);
        }
    }
}
