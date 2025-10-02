using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamers.Features.Shared.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailNotificationsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ReplyId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChatId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatMessages_ReplyId",
                        column: x => x.ReplyId,
                        principalSchema: "public",
                        principalTable: "ChatMessages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    PinnedMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    SettingsId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatSettings",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SlowMode = table.Column<int>(type: "integer", nullable: true),
                    FollowersOnly = table.Column<bool>(type: "boolean", nullable: false),
                    SubscribersOnly = table.Column<bool>(type: "boolean", nullable: false),
                    BannedWords = table.Column<List<string>>(type: "text[]", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Followers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowerStreamerId = table.Column<string>(type: "text", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    FollowedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Followers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PinnedChatMessages",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    PinnedById = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinnedChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PinnedChatMessages_ChatMessages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "public",
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    ChannelBanner = table.Column<string>(type: "text", nullable: true),
                    OfflineStreamBanner = table.Column<string>(type: "text", nullable: true),
                    Instagram = table.Column<string>(type: "text", nullable: true),
                    Youtube = table.Column<string>(type: "text", nullable: true),
                    Discord = table.Column<string>(type: "text", nullable: true),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Permissions = table.Column<int>(type: "integer", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    BroadcasterId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Streamers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    IsLive = table.Column<bool>(type: "boolean", nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FinishedAuth = table.Column<bool>(type: "boolean", nullable: false),
                    Followers = table.Column<long>(type: "bigint", nullable: false),
                    SettingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentStreamId = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streamers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Streamers_Settings_SettingId",
                        column: x => x.SettingId,
                        principalSchema: "public",
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Streams",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamId = table.Column<string>(type: "text", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CurrentViewers = table.Column<long>(type: "bigint", nullable: false),
                    Started = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Streams_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StreamSettings",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamKey = table.Column<string>(type: "text", nullable: false),
                    StreamKeyToken = table.Column<string>(type: "text", nullable: false),
                    StreamUrl = table.Column<string>(type: "text", nullable: false),
                    StreamName = table.Column<string>(type: "text", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreamSettings_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemRoles",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    RoleType = table.Column<int>(type: "integer", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemRoles_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vods",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StreamHls = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Preview = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Views = table.Column<long>(type: "bigint", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration = table.Column<long>(type: "bigint", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vods_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StreamSources",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceType = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreamSources_Streams_StreamId",
                        column: x => x.StreamId,
                        principalSchema: "public",
                        principalTable: "Streams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatId",
                schema: "public",
                table: "ChatMessages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ReplyId",
                schema: "public",
                table: "ChatMessages",
                column: "ReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderId",
                schema: "public",
                table: "ChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_PinnedMessageId",
                schema: "public",
                table: "Chats",
                column: "PinnedMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_SettingsId",
                schema: "public",
                table: "Chats",
                column: "SettingsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_StreamerId",
                schema: "public",
                table: "Chats",
                column: "StreamerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatSettings_StreamerId",
                schema: "public",
                table: "ChatSettings",
                column: "StreamerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowerStreamerId_StreamerId",
                schema: "public",
                table: "Followers",
                columns: new[] { "FollowerStreamerId", "StreamerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Followers_StreamerId",
                schema: "public",
                table: "Followers",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_PinnedChatMessages_MessageId",
                schema: "public",
                table: "PinnedChatMessages",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_PinnedChatMessages_PinnedById",
                schema: "public",
                table: "PinnedChatMessages",
                column: "PinnedById");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_StreamerId",
                schema: "public",
                table: "Profiles",
                column: "StreamerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_BroadcasterId",
                schema: "public",
                table: "Roles",
                column: "BroadcasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_StreamerId",
                schema: "public",
                table: "Roles",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_Streamers_CurrentStreamId",
                schema: "public",
                table: "Streamers",
                column: "CurrentStreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Streamers_SettingId",
                schema: "public",
                table: "Streamers",
                column: "SettingId");

            migrationBuilder.CreateIndex(
                name: "IX_Streamers_UserName",
                schema: "public",
                table: "Streamers",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Streams_StreamerId",
                schema: "public",
                table: "Streams",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_StreamId",
                schema: "public",
                table: "Streams",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StreamSettings_StreamerId",
                schema: "public",
                table: "StreamSettings",
                column: "StreamerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StreamSources_StreamId",
                schema: "public",
                table: "StreamSources",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemRoles_StreamerId",
                schema: "public",
                table: "SystemRoles",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vods_StreamerId",
                schema: "public",
                table: "Vods",
                column: "StreamerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Chats_ChatId",
                schema: "public",
                table: "ChatMessages",
                column: "ChatId",
                principalSchema: "public",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Streamers_SenderId",
                schema: "public",
                table: "ChatMessages",
                column: "SenderId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_ChatSettings_SettingsId",
                schema: "public",
                table: "Chats",
                column: "SettingsId",
                principalSchema: "public",
                principalTable: "ChatSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_PinnedChatMessages_PinnedMessageId",
                schema: "public",
                table: "Chats",
                column: "PinnedMessageId",
                principalSchema: "public",
                principalTable: "PinnedChatMessages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Streamers_StreamerId",
                schema: "public",
                table: "Chats",
                column: "StreamerId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatSettings_Streamers_StreamerId",
                schema: "public",
                table: "ChatSettings",
                column: "StreamerId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_Streamers_FollowerStreamerId",
                schema: "public",
                table: "Followers",
                column: "FollowerStreamerId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_Streamers_StreamerId",
                schema: "public",
                table: "Followers",
                column: "StreamerId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PinnedChatMessages_Streamers_PinnedById",
                schema: "public",
                table: "PinnedChatMessages",
                column: "PinnedById",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Streamers_StreamerId",
                schema: "public",
                table: "Profiles",
                column: "StreamerId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Streamers_BroadcasterId",
                schema: "public",
                table: "Roles",
                column: "BroadcasterId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Streamers_StreamerId",
                schema: "public",
                table: "Roles",
                column: "StreamerId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Streamers_Streams_CurrentStreamId",
                schema: "public",
                table: "Streamers",
                column: "CurrentStreamId",
                principalSchema: "public",
                principalTable: "Streams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Chats_ChatId",
                schema: "public",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Streams_Streamers_StreamerId",
                schema: "public",
                table: "Streams");

            migrationBuilder.DropTable(
                name: "Followers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Profiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StreamSettings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StreamSources",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SystemRoles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Vods",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Chats",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ChatSettings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PinnedChatMessages",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ChatMessages",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Streamers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Streams",
                schema: "public");
        }
    }
}
