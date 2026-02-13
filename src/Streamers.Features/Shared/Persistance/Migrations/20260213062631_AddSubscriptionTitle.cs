using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "public",
                table: "Subscriptions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                schema: "public",
                table: "Subscriptions");
        }
    }
}
