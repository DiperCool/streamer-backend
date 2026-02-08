using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddPartnerStreamer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Partners_StreamerId",
                schema: "public",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Customers_StreamerId",
                schema: "public",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_StreamerId",
                schema: "public",
                table: "Partners",
                column: "StreamerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_StreamerId",
                schema: "public",
                table: "Customers",
                column: "StreamerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Partners_StreamerId",
                schema: "public",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Customers_StreamerId",
                schema: "public",
                table: "Customers");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_StreamerId",
                schema: "public",
                table: "Partners",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_StreamerId",
                schema: "public",
                table: "Customers",
                column: "StreamerId");
        }
    }
}
