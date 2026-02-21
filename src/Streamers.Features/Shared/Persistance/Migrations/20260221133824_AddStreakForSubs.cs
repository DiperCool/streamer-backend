using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddStreakForSubs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStreak",
                schema: "public",
                table: "Subscriptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStreak",
                schema: "public",
                table: "Subscriptions");
        }
    }
}
