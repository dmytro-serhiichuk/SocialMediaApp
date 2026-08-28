using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp1.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddImageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "postImages");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "postImages");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "postImages");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "postImages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_postImages_ImageId",
                table: "postImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_images_StorageKey",
                table: "images",
                column: "StorageKey",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_postImages_images_ImageId",
                table: "postImages",
                column: "ImageId",
                principalTable: "images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_postImages_images_ImageId",
                table: "postImages");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropIndex(
                name: "IX_postImages_ImageId",
                table: "postImages");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "postImages");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "postImages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "postImages",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "postImages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
