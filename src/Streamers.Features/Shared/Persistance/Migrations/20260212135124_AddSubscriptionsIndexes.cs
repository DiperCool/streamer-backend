using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionsIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_StripeSubscriptionId",
                schema: "public",
                table: "Subscriptions",
                column: "StripeSubscriptionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_StripeSubscriptionId",
                schema: "public",
                table: "Subscriptions");
        }
    }
}
