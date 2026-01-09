using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityToBundle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityAvailable",
                table: "BundleItems");

            migrationBuilder.AddColumn<int>(
                name: "QuantityAvailable",
                table: "Bundles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityAvailable",
                table: "Bundles");

            migrationBuilder.AddColumn<int>(
                name: "QuantityAvailable",
                table: "BundleItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
