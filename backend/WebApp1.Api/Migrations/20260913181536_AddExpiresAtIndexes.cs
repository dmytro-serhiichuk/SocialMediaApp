using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddExpiresAtIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_pendingRegistrations_ExpiresAt",
                table: "pendingRegistrations",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_passwordResets_ExpiresAt",
                table: "passwordResets",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_newLoginSecurityRequests_ExpiresAt",
                table: "newLoginSecurityRequests",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_emailChangeRequests_ExpiresAt",
                table: "emailChangeRequests",
                column: "ExpiresAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_pendingRegistrations_ExpiresAt",
                table: "pendingRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_passwordResets_ExpiresAt",
                table: "passwordResets");

            migrationBuilder.DropIndex(
                name: "IX_newLoginSecurityRequests_ExpiresAt",
                table: "newLoginSecurityRequests");

            migrationBuilder.DropIndex(
                name: "IX_emailChangeRequests_ExpiresAt",
                table: "emailChangeRequests");
        }
    }
}
