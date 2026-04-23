using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceShopper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckoutPromotionTargetsAndOrderPromotionRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderPromotion",
                schema: "spaceshopper",
                table: "OrderPromotion");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "spaceshopper",
                table: "OrderShipping",
                newName: "ShippingMethodName");

            migrationBuilder.RenameColumn(
                name: "Code",
                schema: "spaceshopper",
                table: "OrderShipping",
                newName: "ShippingMethodCode");

            migrationBuilder.RenameColumn(
                name: "Code",
                schema: "spaceshopper",
                table: "OrderPromotion",
                newName: "PromotionCode");

            migrationBuilder.AddColumn<bool>(
                name: "IsShippingDiscount",
                schema: "spaceshopper",
                table: "Promotion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "spaceshopper",
                table: "OrderPromotion",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<bool>(
                name: "IsShippingDiscount",
                schema: "spaceshopper",
                table: "OrderPromotion",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OrderCode",
                schema: "spaceshopper",
                table: "Order",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                schema: "spaceshopper",
                table: "Order",
                type: "text",
                nullable: false,
                defaultValue: "Unpaid");

            migrationBuilder.AddColumn<decimal>(
                name: "ProductDiscountAmount",
                schema: "spaceshopper",
                table: "Order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingDiscountAmount",
                schema: "spaceshopper",
                table: "Order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderPromotion",
                schema: "spaceshopper",
                table: "OrderPromotion",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MethodShipping",
                schema: "spaceshopper",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    EstimatedDaysMin = table.Column<int>(type: "integer", nullable: true),
                    EstimatedDaysMax = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MethodShipping", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderPromotion_OrderId_IsShippingDiscount",
                schema: "spaceshopper",
                table: "OrderPromotion",
                columns: new[] { "OrderId", "IsShippingDiscount" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MethodShipping",
                schema: "spaceshopper");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderPromotion",
                schema: "spaceshopper",
                table: "OrderPromotion");

            migrationBuilder.DropIndex(
                name: "IX_OrderPromotion_OrderId_IsShippingDiscount",
                schema: "spaceshopper",
                table: "OrderPromotion");

            migrationBuilder.DropColumn(
                name: "IsShippingDiscount",
                schema: "spaceshopper",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "spaceshopper",
                table: "OrderPromotion");

            migrationBuilder.DropColumn(
                name: "IsShippingDiscount",
                schema: "spaceshopper",
                table: "OrderPromotion");

            migrationBuilder.DropColumn(
                name: "OrderCode",
                schema: "spaceshopper",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                schema: "spaceshopper",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ProductDiscountAmount",
                schema: "spaceshopper",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShippingDiscountAmount",
                schema: "spaceshopper",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "ShippingMethodName",
                schema: "spaceshopper",
                table: "OrderShipping",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ShippingMethodCode",
                schema: "spaceshopper",
                table: "OrderShipping",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "PromotionCode",
                schema: "spaceshopper",
                table: "OrderPromotion",
                newName: "Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderPromotion",
                schema: "spaceshopper",
                table: "OrderPromotion",
                column: "OrderId");
        }
    }
}
