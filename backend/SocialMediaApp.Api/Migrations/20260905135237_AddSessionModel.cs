using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SocialMediaApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RefreshTokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RefreshTokenExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PreviousRefreshTokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PreviousTokenGraceUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastUserAgent = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LastIpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sessions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sessions_PreviousRefreshTokenHash",
                table: "sessions",
                column: "PreviousRefreshTokenHash");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_RefreshTokenExpiresAt",
                table: "sessions",
                column: "RefreshTokenExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_sessions_RefreshTokenHash",
                table: "sessions",
                column: "RefreshTokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sessions_UserId",
                table: "sessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sessions");
        }
    }
}
