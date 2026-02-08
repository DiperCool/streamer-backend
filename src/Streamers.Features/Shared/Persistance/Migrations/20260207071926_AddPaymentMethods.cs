using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StripePaymentMethodId = table.Column<string>(type: "text", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    CardBrand = table.Column<string>(type: "text", nullable: false),
                    CardLast4 = table.Column<string>(type: "text", nullable: false),
                    CardExpMonth = table.Column<long>(type: "bigint", nullable: false),
                    CardExpYear = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_StreamerId",
                schema: "public",
                table: "PaymentMethods",
                column: "StreamerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentMethods",
                schema: "public");
        }
    }
}
