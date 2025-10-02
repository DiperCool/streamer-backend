using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddVodSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasUnreadNotifications",
                schema: "public",
                table: "Streamers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    StreamerLive = table.Column<bool>(type: "boolean", nullable: false),
                    UserFollowed = table.Column<bool>(type: "boolean", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationSettings_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VodSettings",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    VodEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VodSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VodSettings_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettings_StreamerId",
                schema: "public",
                table: "NotificationSettings",
                column: "StreamerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VodSettings_StreamerId",
                schema: "public",
                table: "VodSettings",
                column: "StreamerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationSettings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "VodSettings",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "HasUnreadNotifications",
                schema: "public",
                table: "Streamers");
        }
    }
}
