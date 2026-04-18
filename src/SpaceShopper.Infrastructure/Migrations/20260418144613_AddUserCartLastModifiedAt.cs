using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceShopper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCartLastModifiedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                schema: "spaceshopper",
                table: "UserCart",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                schema: "spaceshopper",
                table: "UserCart");
        }
    }
}
