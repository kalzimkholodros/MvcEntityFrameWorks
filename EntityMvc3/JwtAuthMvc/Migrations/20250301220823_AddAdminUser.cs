using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JwtAuthMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Balance", "Email", "IsPremium", "Password", "PremiumEndDate", "Role", "Username" },
                values: new object[] { 1, 0m, "admin@example.com", true, "$2a$11$PcojnoRDaCx.DXUMBb8FguyIewlS6f/hG5r7uagSMPToMeqGhPXsS", new DateTime(2099, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
