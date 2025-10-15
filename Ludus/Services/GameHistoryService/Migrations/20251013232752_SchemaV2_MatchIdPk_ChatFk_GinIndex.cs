using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameHistoryService.Migrations
{
    /// <inheritdoc />
    public partial class SchemaV2_MatchIdPk_ChatFk_GinIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_GameMatchId",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "GameMatchId",
                table: "ChatMessages",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_GameMatchId_SenderUserId_SentAt_Message",
                table: "ChatMessages",
                columns: new[] { "GameMatchId", "SenderUserId", "SentAt", "Message" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_GameMatchId_SenderUserId_SentAt_Message",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<string>(
                name: "GameMatchId",
                table: "ChatMessages",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_GameMatchId",
                table: "ChatMessages",
                column: "GameMatchId");
        }
    }
}
