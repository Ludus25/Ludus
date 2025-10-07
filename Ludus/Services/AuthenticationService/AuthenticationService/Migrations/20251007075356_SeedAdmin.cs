using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationService.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "mlb", "name", "surname" },
                values: new object[] { "1", 0, "STATIC-CONCURRENCY-STAMP-0001", "ludusadm1@gmail.com", true, false, null, "LUDUSADM1@GMAIL.COM", "LUDUSADM1@GMAIL.COM", "$2y$10$IWHM/QhahGfo86siqBCFrOeqtqjafIFoT6RG3vaiPdgnuJWnESStW", null, false, "STATIC-SECURITY-STAMP-0001", true, "ludusadm1@gmail.com", "1203998710890", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "cc10e60a-8052-4227-8a8c-d1c22e01480e", "1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "cc10e60a-8052-4227-8a8c-d1c22e01480e", "1" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1");
        }
    }
}
