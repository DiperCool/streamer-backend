using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddStreamerAndUserTypeInSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_StreamerId",
                schema: "public",
                table: "Subscriptions",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                schema: "public",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Streamers_StreamerId",
                schema: "public",
                table: "Subscriptions",
                column: "StreamerId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Streamers_UserId",
                schema: "public",
                table: "Subscriptions",
                column: "UserId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Streamers_StreamerId",
                schema: "public",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Streamers_UserId",
                schema: "public",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_StreamerId",
                schema: "public",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_UserId",
                schema: "public",
                table: "Subscriptions");
        }
    }
}
