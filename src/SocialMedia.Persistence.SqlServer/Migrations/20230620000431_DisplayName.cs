using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMedia.Persistence.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class DisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfile",
                schema: "SocialMedia",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserId",
                schema: "SocialMedia",
                table: "Post",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_UserProfile_UserId",
                schema: "SocialMedia",
                table: "Post",
                column: "UserId",
                principalSchema: "SocialMedia",
                principalTable: "UserProfile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_UserProfile_UserId",
                schema: "SocialMedia",
                table: "Post");

            migrationBuilder.DropTable(
                name: "UserProfile",
                schema: "SocialMedia");

            migrationBuilder.DropIndex(
                name: "IX_Post_UserId",
                schema: "SocialMedia",
                table: "Post");
        }
    }
}
