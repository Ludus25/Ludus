using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameHistoryService.Migrations
{
    public partial class AddEmailSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Add as NULLable
            migrationBuilder.AddColumn<string[]>(
                name: "PlayerEmails",
                table: "GameHistories",
                type: "text[]",
                nullable: true);

            // 2) Backfill existing rows to empty array
            migrationBuilder.Sql("""
                UPDATE "GameHistories"
                SET "PlayerEmails" = ARRAY[]::text[]
                WHERE "PlayerEmails" IS NULL;
            """);

            // 3) Enforce NOT NULL (no default)
            migrationBuilder.AlterColumn<string[]>(
                name: "PlayerEmails",
                table: "GameHistories",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

            // 4) GIN index
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_class c
                        JOIN pg_namespace n ON n.oid = c.relnamespace
                        WHERE c.relname = 'IX_GameHistories_PlayerEmails' AND n.nspname = 'public'
                    ) THEN
                        CREATE INDEX "IX_GameHistories_PlayerEmails"
                        ON "GameHistories" USING GIN ("PlayerEmails");
                    END IF;
                END$$;
            """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM pg_class c
                        JOIN pg_namespace n ON n.oid = c.relnamespace
                        WHERE c.relname = 'IX_GameHistories_PlayerEmails' AND n.nspname = 'public'
                    ) THEN
                        DROP INDEX "IX_GameHistories_PlayerEmails";
                    END IF;
                END$$;
            """);

            migrationBuilder.DropColumn(
                name: "PlayerEmails",
                table: "GameHistories");
        }
    }
}
