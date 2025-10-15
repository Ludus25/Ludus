using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameHistoryService.Migrations
{
    /// <inheritdoc />
    public partial class PlayerUserIdsArray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "GameHistories"
                ALTER COLUMN "PlayerUserIds" TYPE text[]
                USING string_to_array("PlayerUserIds", ',');
            """);

            migrationBuilder.CreateIndex(
                name: "IX_GameHistories_PlayerUserIds",
                table: "GameHistories",
                column: "PlayerUserIds")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameHistories_PlayerUserIds",
                table: "GameHistories");

            migrationBuilder.Sql("""
                ALTER TABLE "GameHistories"
                ALTER COLUMN "PlayerUserIds" TYPE text
                USING array_to_string("PlayerUserIds", ',');
            """);
        }
    }
}
