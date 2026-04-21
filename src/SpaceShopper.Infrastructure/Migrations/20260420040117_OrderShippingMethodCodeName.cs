using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceShopper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderShippingMethodCodeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShippingMethod",
                schema: "spaceshopper",
                table: "OrderShipping",
                newName: "Name");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "spaceshopper",
                table: "Promotion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "spaceshopper",
                table: "OrderShipping",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "spaceshopper",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "spaceshopper",
                table: "OrderShipping");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "spaceshopper",
                table: "OrderShipping",
                newName: "ShippingMethod");
        }
    }
}
