using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddBos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bots",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    StreamVideoUrl = table.Column<string>(type: "text", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bots_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bots_StreamerId",
                schema: "public",
                table: "Bots",
                column: "StreamerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bots",
                schema: "public");
        }
    }
}
