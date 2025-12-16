using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceShopper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.CreateTable(
                name: "Product",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    idClone = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    price_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    price_currency = table.Column<string>(type: "text", nullable: false),
                    real_price_amount = table.Column<decimal>(type: "numeric", nullable: true),
                    real_price_currency = table.Column<string>(type: "text", nullable: true),
                    PriceUsd = table.Column<decimal>(type: "numeric", nullable: true),
                    RatingAverage = table.Column<decimal>(type: "numeric", nullable: false),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product",
                schema: "catalog");
        }
    }
}
