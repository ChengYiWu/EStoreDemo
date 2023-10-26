using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyProductAndProductItemCacadeSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_User_CreatedBy",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_User_UpdatedBy",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductItem_User_CreatedBy",
                table: "ProductItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductItem_User_UpdatedBy",
                table: "ProductItem");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_User_CreatedBy",
                table: "Product",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_User_UpdatedBy",
                table: "Product",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItem_User_CreatedBy",
                table: "ProductItem",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItem_User_UpdatedBy",
                table: "ProductItem",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_User_CreatedBy",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_User_UpdatedBy",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductItem_User_CreatedBy",
                table: "ProductItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductItem_User_UpdatedBy",
                table: "ProductItem");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_User_CreatedBy",
                table: "Product",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_User_UpdatedBy",
                table: "Product",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItem_User_CreatedBy",
                table: "ProductItem",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItem_User_UpdatedBy",
                table: "ProductItem",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
