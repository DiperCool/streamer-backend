using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFollowedNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserFollowedNotification_StreamerId",
                schema: "public",
                table: "Notifications",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserFollowedNotification_StreamerId",
                schema: "public",
                table: "Notifications",
                column: "UserFollowedNotification_StreamerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Streamers_UserFollowedNotification_StreamerId",
                schema: "public",
                table: "Notifications",
                column: "UserFollowedNotification_StreamerId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Streamers_UserFollowedNotification_StreamerId",
                schema: "public",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserFollowedNotification_StreamerId",
                schema: "public",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserFollowedNotification_StreamerId",
                schema: "public",
                table: "Notifications");
        }
    }
}
