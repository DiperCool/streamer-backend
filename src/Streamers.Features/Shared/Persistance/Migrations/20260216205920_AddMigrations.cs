using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PlatformFee = table.Column<decimal>(type: "numeric", nullable: false),
                    StreamerNet = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StripeInvoiceUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Streamers_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_StreamerId",
                schema: "public",
                table: "Transactions",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                schema: "public",
                table: "Transactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "public");
        }
    }
}
