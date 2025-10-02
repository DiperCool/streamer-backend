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
                name: "Categories",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

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
                name: "AnalyticsItems",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BannedUsers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    BroadcasterId = table.Column<string>(type: "text", nullable: false),
                    BannedById = table.Column<string>(type: "text", nullable: false),
                    BannedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BannedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    JobId = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannedUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banners",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
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
                name: "Notifications",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Seen = table.Column<bool>(type: "boolean", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: true),
                    UserFollowedNotification_StreamerId = table.Column<string>(type: "text", nullable: true),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    StreamerLive = table.Column<bool>(type: "boolean", nullable: false),
                    UserFollowed = table.Column<bool>(type: "boolean", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
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
                    HasUnreadNotifications = table.Column<bool>(type: "boolean", nullable: false),
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
                    Language = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Duration = table.Column<double>(type: "double precision", nullable: true),
                    Preview = table.Column<string>(type: "text", nullable: true),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Streams_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "public",
                        principalTable: "Categories",
                        principalColumn: "Id");
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
                    Type = table.Column<int>(type: "integer", nullable: false),
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
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Language = table.Column<string>(type: "text", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vods_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "public",
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vods_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VodSettings",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamerId = table.Column<string>(type: "text", nullable: false),
                    VodEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    OriginalVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VodSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VodSettings_Streamers_StreamerId",
                        column: x => x.StreamerId,
                        principalSchema: "public",
                        principalTable: "Streamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_AnalyticsItems_StreamerId",
                schema: "public",
                table: "AnalyticsItems",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_BannedUsers_BannedById",
                schema: "public",
                table: "BannedUsers",
                column: "BannedById");

            migrationBuilder.CreateIndex(
                name: "IX_BannedUsers_BroadcasterId",
                schema: "public",
                table: "BannedUsers",
                column: "BroadcasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BannedUsers_UserId",
                schema: "public",
                table: "BannedUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_StreamerId",
                schema: "public",
                table: "Banners",
                column: "StreamerId");

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
                name: "IX_Notifications_StreamerId",
                schema: "public",
                table: "Notifications",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserFollowedNotification_StreamerId",
                schema: "public",
                table: "Notifications",
                column: "UserFollowedNotification_StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                schema: "public",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSettings_StreamerId",
                schema: "public",
                table: "NotificationSettings",
                column: "StreamerId",
                unique: true);

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
                name: "IX_Streams_CategoryId",
                schema: "public",
                table: "Streams",
                column: "CategoryId");

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
                name: "IX_StreamTag_TagsId",
                schema: "public",
                table: "StreamTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemRoles_StreamerId",
                schema: "public",
                table: "SystemRoles",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_TagVod_VodsId",
                schema: "public",
                table: "TagVod",
                column: "VodsId");

            migrationBuilder.CreateIndex(
                name: "IX_Vods_CategoryId",
                schema: "public",
                table: "Vods",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Vods_StreamerId",
                schema: "public",
                table: "Vods",
                column: "StreamerId");

            migrationBuilder.CreateIndex(
                name: "IX_VodSettings_StreamerId",
                schema: "public",
                table: "VodSettings",
                column: "StreamerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnalyticsItems_Streamers_StreamerId",
                schema: "public",
                table: "AnalyticsItems",
                column: "StreamerId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BannedUsers_Streamers_BannedById",
                schema: "public",
                table: "BannedUsers",
                column: "BannedById",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BannedUsers_Streamers_BroadcasterId",
                schema: "public",
                table: "BannedUsers",
                column: "BroadcasterId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BannedUsers_Streamers_UserId",
                schema: "public",
                table: "BannedUsers",
                column: "UserId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Banners_Streamers_StreamerId",
                schema: "public",
                table: "Banners",
                column: "StreamerId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Notifications_Streamers_StreamerId",
                schema: "public",
                table: "Notifications",
                column: "StreamerId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Streamers_UserFollowedNotification_StreamerId",
                schema: "public",
                table: "Notifications",
                column: "UserFollowedNotification_StreamerId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Streamers_UserId",
                schema: "public",
                table: "Notifications",
                column: "UserId",
                principalSchema: "public",
                principalTable: "Streamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationSettings_Streamers_StreamerId",
                schema: "public",
                table: "NotificationSettings",
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
                name: "FK_ChatMessages_Streamers_SenderId",
                schema: "public",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Streamers_StreamerId",
                schema: "public",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatSettings_Streamers_StreamerId",
                schema: "public",
                table: "ChatSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_PinnedChatMessages_Streamers_PinnedById",
                schema: "public",
                table: "PinnedChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Streams_Streamers_StreamerId",
                schema: "public",
                table: "Streams");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Chats_ChatId",
                schema: "public",
                table: "ChatMessages");

            migrationBuilder.DropTable(
                name: "AnalyticsItems",
                schema: "public");

            migrationBuilder.DropTable(
                name: "BannedUsers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Banners",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Followers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "public");

            migrationBuilder.DropTable(
                name: "NotificationSettings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Profiles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StreamInfoTag",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StreamSettings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StreamSources",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StreamTag",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SystemRoles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TagVod",
                schema: "public");

            migrationBuilder.DropTable(
                name: "VodSettings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StreamInfos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Vods",
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

            migrationBuilder.DropTable(
                name: "Categories",
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
        }
    }
}
