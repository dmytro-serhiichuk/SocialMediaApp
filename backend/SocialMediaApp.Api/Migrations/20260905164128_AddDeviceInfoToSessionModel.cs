using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMediaApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceInfoToSessionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUserAgent",
                table: "sessions");

            migrationBuilder.AddColumn<string>(
                name: "LastDeviceInfo",
                table: "sessions",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDeviceInfo",
                table: "sessions");

            migrationBuilder.AddColumn<string>(
                name: "LastUserAgent",
                table: "sessions",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
