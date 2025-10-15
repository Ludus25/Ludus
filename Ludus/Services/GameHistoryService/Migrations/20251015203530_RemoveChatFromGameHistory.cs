using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameHistoryService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveChatFromGameHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameMatchId = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    SenderUserId = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_GameHistories_GameMatchId",
                        column: x => x.GameMatchId,
                        principalTable: "GameHistories",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_GameMatchId_SenderUserId_SentAt_Message",
                table: "ChatMessages",
                columns: new[] { "GameMatchId", "SenderUserId", "SentAt", "Message" },
                unique: true);
        }
    }
}
