using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMediaApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailChangeRequestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "emailChangeRequests",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    NewEmail = table.Column<string>(type: "text", nullable: false),
                    TokenHash = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emailChangeRequests", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_emailChangeRequests_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_emailChangeRequests_TokenHash",
                table: "emailChangeRequests",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "emailChangeRequests");
        }
    }
}
