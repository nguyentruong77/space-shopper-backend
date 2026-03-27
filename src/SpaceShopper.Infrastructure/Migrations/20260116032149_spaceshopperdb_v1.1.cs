using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceShopper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class spaceshopperdb_v11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "spaceshopper",
                table: "ProductReview");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "spaceshopper",
                table: "ProductReview");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                schema: "spaceshopper",
                table: "ProductReview");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "spaceshopper",
                table: "ProductReview",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "spaceshopper",
                table: "ProductReview",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                schema: "spaceshopper",
                table: "ProductReview",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
