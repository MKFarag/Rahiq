using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDisabled", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "019b2c2d-01dc-7a64-a484-4d1a62e4a725", "019b2c2d-01dc-7a64-a484-4d1b3c6d8f0f", false, false, "Admin", "ADMIN" },
                    { "019b2c2d-01dc-7a64-a484-4d1ce2622b71", "019b2c2d-01dc-7a64-a484-4d1ddafd5d61", true, false, "Customer", "CUSTOMER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsDisabled", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "019b2c2d-01dc-7a64-a484-4d188b6d5173", 0, "019b2c2d-01dc-7a64-a484-4d19325bbc67", "Farag@Rahiq.com", true, "Rahiq", false, "Admin", false, null, "FARAG@RAHIQ.COM", "FARAG@RAHIQ.COM", "AQAAAAIAAYagAAAAEMxwVAaoCulBCKEsmwW7XyCWBBEsCSNdQO2Mbs0chBARezA5gRK1T4tihgXGJCmbKA==", null, false, "B1BBD18BC4BE4EE592C378A32EF8B935", false, "Farag@Rahiq.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "019b2c2d-01dc-7a64-a484-4d1a62e4a725", "019b2c2d-01dc-7a64-a484-4d188b6d5173" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019b2c2d-01dc-7a64-a484-4d1ce2622b71");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "019b2c2d-01dc-7a64-a484-4d1a62e4a725", "019b2c2d-01dc-7a64-a484-4d188b6d5173" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019b2c2d-01dc-7a64-a484-4d1a62e4a725");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019b2c2d-01dc-7a64-a484-4d188b6d5173");
        }
    }
}
