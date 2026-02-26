using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddModeratorActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModeratorActionTypes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ModeratorId = table.Column<string>(type: "text", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    TargetUserId = table.Column<string>(type: "text", nullable: true),
                    BannedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    NewChatMode = table.Column<string>(type: "text", nullable: true),
                    ChatMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    NewCategory = table.Column<string>(type: "text", nullable: true),
                    NewLanguage = table.Column<string>(type: "text", nullable: true),
                    NewStreamName = table.Column<string>(type: "text", nullable: true),
                    UnbanAction_TargetUserId = table.Column<string>(type: "text", nullable: true),
                    UnpinAction_ChatMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeratorActionTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModeratorActionTypes_ChatMessages_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalSchema: "public",
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModeratorActionTypes_ChatMessages_UnpinAction_ChatMessageId",
                        column: x => x.UnpinAction_ChatMessageId,
                        principalSchema: "public",
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModeratorActionTypes_Streamers_ModeratorId",
                        column: x => x.ModeratorId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModeratorActionTypes_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModeratorActionTypes_Streamers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModeratorActionTypes_Streamers_UnbanAction_TargetUserId",
                        column: x => x.UnbanAction_TargetUserId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorActionTypes_ChatMessageId",
                schema: "public",
                table: "ModeratorActionTypes",
                column: "ChatMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorActionTypes_ModeratorId",
                schema: "public",
                table: "ModeratorActionTypes",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorActionTypes_StreamerId",
                schema: "public",
                table: "ModeratorActionTypes",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorActionTypes_TargetUserId",
                schema: "public",
                table: "ModeratorActionTypes",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorActionTypes_UnbanAction_TargetUserId",
                schema: "public",
                table: "ModeratorActionTypes",
                column: "UnbanAction_TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorActionTypes_UnpinAction_ChatMessageId",
                schema: "public",
                table: "ModeratorActionTypes",
                column: "UnpinAction_ChatMessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModeratorActionTypes",
                schema: "public");
        }
    }
}
