using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTagToUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserTag",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_users_UserTag",
                table: "users",
                column: "UserTag",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_UserTag",
                table: "users");

            migrationBuilder.DropColumn(
                name: "UserTag",
                table: "users");
        }
    }
}
