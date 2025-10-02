using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                schema: "public",
                table: "Vods",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                schema: "public",
                table: "Vods",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                schema: "public",
                table: "Streams",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                schema: "public",
                table: "Streams",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StreamInfos",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Language = table.Column<string>(type: "text", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreamInfos_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "public",
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StreamInfos_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StreamInfoTag",
                schema: "public",
                columns: table => new
                {
                    StreamInfosId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamInfoTag", x => new { x.StreamInfosId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_StreamInfoTag_StreamInfos_StreamInfosId",
                        column: x => x.StreamInfosId,
                        principalSchema: "public",
                        principalTable: "StreamInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreamInfoTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalSchema: "public",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StreamTag",
                schema: "public",
                columns: table => new
                {
                    StreamsId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamTag", x => new { x.StreamsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_StreamTag_Streams_StreamsId",
                        column: x => x.StreamsId,
                        principalSchema: "public",
                        principalTable: "Streams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreamTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalSchema: "public",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagVod",
                schema: "public",
                columns: table => new
                {
                    TagsId = table.Column<Guid>(type: "uuid", nullable: false),
                    VodsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagVod", x => new { x.TagsId, x.VodsId });
                    table.ForeignKey(
                        name: "FK_TagVod_Tags_TagsId",
                        column: x => x.TagsId,
                        principalSchema: "public",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagVod_Vods_VodsId",
                        column: x => x.VodsId,
                        principalSchema: "public",
                        principalTable: "Vods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vods_CategoryId",
                schema: "public",
                table: "Vods",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_CategoryId",
                schema: "public",
                table: "Streams",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamInfos_CategoryId",
                schema: "public",
                table: "StreamInfos",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamInfos_StreamerId",
                schema: "public",
                table: "StreamInfos",
                column: "StreamerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StreamInfoTag_TagsId",
                schema: "public",
                table: "StreamInfoTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_StreamTag_TagsId",
                schema: "public",
                table: "StreamTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_TagVod_VodsId",
                schema: "public",
                table: "TagVod",
                column: "VodsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Streams_Categories_CategoryId",
                schema: "public",
                table: "Streams",
                column: "CategoryId",
                principalSchema: "public",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vods_Categories_CategoryId",
                schema: "public",
                table: "Vods",
                column: "CategoryId",
                principalSchema: "public",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Streams_Categories_CategoryId",
                schema: "public",
                table: "Streams");

            migrationBuilder.DropForeignKey(
                name: "FK_Vods_Categories_CategoryId",
                schema: "public",
                table: "Vods");

            migrationBuilder.DropTable(
                name: "StreamInfoTag",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StreamTag",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TagVod",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StreamInfos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Vods_CategoryId",
                schema: "public",
                table: "Vods");

            migrationBuilder.DropIndex(
                name: "IX_Streams_CategoryId",
                schema: "public",
                table: "Streams");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "public",
                table: "Vods");

            migrationBuilder.DropColumn(
                name: "Language",
                schema: "public",
                table: "Vods");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "public",
                table: "Streams");

            migrationBuilder.DropColumn(
                name: "Language",
                schema: "public",
                table: "Streams");
        }
    }
}
