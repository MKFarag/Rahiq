using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionsToAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "bundle:add", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 2, "permissions", "bundle:modify", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 3, "permissions", "bundle:activation", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 4, "permissions", "category:add", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 5, "permissions", "category:modify", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 6, "permissions", "category:delete", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 7, "permissions", "order:read", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 8, "permissions", "order:add", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 9, "permissions", "order:cancel", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 10, "permissions", "order:change-status", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 11, "permissions", "payment:verify", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 12, "permissions", "product:add", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 13, "permissions", "product:modify", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 14, "permissions", "product:discount", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 15, "permissions", "product:change-status", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 16, "permissions", "role:read", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 17, "permissions", "role:add", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 18, "permissions", "role:modify", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 19, "permissions", "role:toggle-status", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 20, "permissions", "shipping:read", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 21, "permissions", "shipping:assign-details", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 22, "permissions", "shipping:modify", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 23, "permissions", "shipping:delete", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 24, "permissions", "type:add", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 25, "permissions", "type:modify", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" },
                    { 26, "permissions", "type:delete", "019b2c2d-01dc-7a64-a484-4d1a62e4a725" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 26);
        }
    }
}
