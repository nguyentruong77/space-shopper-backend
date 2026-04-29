using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceShopper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDetailIsReviewed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                schema: "spaceshopper",
                table: "OrderDetail",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReviewed",
                schema: "spaceshopper",
                table: "OrderDetail");
        }
    }
}
