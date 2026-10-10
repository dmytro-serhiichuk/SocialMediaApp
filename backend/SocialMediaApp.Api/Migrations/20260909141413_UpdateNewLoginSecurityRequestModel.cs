using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SocialMediaApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNewLoginSecurityRequestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_newLoginSecurityRequests",
                table: "newLoginSecurityRequests");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "newLoginSecurityRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "newLoginSecurityRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_newLoginSecurityRequests",
                table: "newLoginSecurityRequests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_newLoginSecurityRequests_SessionId",
                table: "newLoginSecurityRequests",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_newLoginSecurityRequests_UserId",
                table: "newLoginSecurityRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_newLoginSecurityRequests_sessions_SessionId",
                table: "newLoginSecurityRequests",
                column: "SessionId",
                principalTable: "sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_newLoginSecurityRequests_sessions_SessionId",
                table: "newLoginSecurityRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_newLoginSecurityRequests",
                table: "newLoginSecurityRequests");

            migrationBuilder.DropIndex(
                name: "IX_newLoginSecurityRequests_SessionId",
                table: "newLoginSecurityRequests");

            migrationBuilder.DropIndex(
                name: "IX_newLoginSecurityRequests_UserId",
                table: "newLoginSecurityRequests");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "newLoginSecurityRequests");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "newLoginSecurityRequests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_newLoginSecurityRequests",
                table: "newLoginSecurityRequests",
                column: "UserId");
        }
    }
}
