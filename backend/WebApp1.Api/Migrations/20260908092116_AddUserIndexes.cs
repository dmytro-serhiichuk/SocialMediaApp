using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_users_UserTag",
                table: "users",
                newName: "IX_Users_UserTag");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "users",
                newName: "IX_Users_Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Users_UserTag",
                table: "users",
                newName: "IX_users_UserTag");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "users",
                newName: "IX_users_Email");
        }
    }
}
