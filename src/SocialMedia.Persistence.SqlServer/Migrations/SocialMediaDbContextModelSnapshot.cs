﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SocialMedia.Persistence.SqlServer;

#nullable disable

namespace SocialMedia.Persistence.SqlServer.Migrations
{
    [DbContext(typeof(SocialMediaDbContext))]
    partial class SocialMediaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("SocialMedia")
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SocialMedia.Persistence.SqlServer.Models.PostContentData", b =>
                {
                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PostId");

                    b.ToTable("PostContent", "SocialMedia");
                });

            modelBuilder.Entity("SocialMedia.Persistence.SqlServer.Models.PostData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Post", "SocialMedia");
                });

            modelBuilder.Entity("SocialMedia.Persistence.SqlServer.Models.UserProfileData", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("UserId");

                    b.ToTable("UserProfile", "SocialMedia");
                });

            modelBuilder.Entity("SocialMedia.Persistence.SqlServer.Models.PostContentData", b =>
                {
                    b.HasOne("SocialMedia.Persistence.SqlServer.Models.PostData", "Post")
                        .WithOne("Content")
                        .HasForeignKey("SocialMedia.Persistence.SqlServer.Models.PostContentData", "PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("SocialMedia.Persistence.SqlServer.Models.PostData", b =>
                {
                    b.HasOne("SocialMedia.Persistence.SqlServer.Models.UserProfileData", "UserProfile")
                        .WithMany("Posts")
                        .HasForeignKey("UserId");

                    b.Navigation("UserProfile");
                });

            modelBuilder.Entity("SocialMedia.Persistence.SqlServer.Models.PostData", b =>
                {
                    b.Navigation("Content")
                        .IsRequired();
                });

            modelBuilder.Entity("SocialMedia.Persistence.SqlServer.Models.UserProfileData", b =>
                {
                    b.Navigation("Posts");
                });
#pragma warning restore 612, 618
        }
    }
}
