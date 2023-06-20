using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMedia.Persistence.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class DisplayNameFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_UserProfile_UserId",
                schema: "SocialMedia",
                table: "Post");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "SocialMedia",
                table: "Post",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_UserProfile_UserId",
                schema: "SocialMedia",
                table: "Post",
                column: "UserId",
                principalSchema: "SocialMedia",
                principalTable: "UserProfile",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_UserProfile_UserId",
                schema: "SocialMedia",
                table: "Post");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "SocialMedia",
                table: "Post",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

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
    }
}
