using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddPartner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SubscriptionEnabled",
                schema: "public",
                table: "Streamers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    StripeCustomerId = table.Column<string>(type: "text", nullable: true),
                    StripeCustomerCreationStatus = table.Column<int>(type: "integer", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partners",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    StripeAccountId = table.Column<string>(type: "text", nullable: true),
                    StripeOnboardingStatus = table.Column<int>(type: "integer", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partners_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    StripeProductId = table.Column<string>(type: "text", nullable: false),
                    StripePriceId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StreamerId1 = table.Column<string>(type: "text", nullable: true),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlans_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionPlans_Streamers_StreamerId1",
                        column: x => x.StreamerId1,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_StreamerId",
                schema: "public",
                table: "Customers",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_StreamerId",
                schema: "public",
                table: "Partners",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_StreamerId",
                schema: "public",
                table: "SubscriptionPlans",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_StreamerId1",
                schema: "public",
                table: "SubscriptionPlans",
                column: "StreamerId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Partners",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "SubscriptionEnabled",
                schema: "public",
                table: "Streamers");
        }
    }
}
