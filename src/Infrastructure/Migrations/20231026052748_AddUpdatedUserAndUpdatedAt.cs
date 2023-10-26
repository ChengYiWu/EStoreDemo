using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedUserAndUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "ProductItem",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ProductItem",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Product",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Product",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductItem_UpdatedBy",
                table: "ProductItem",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Product_UpdatedBy",
                table: "Product",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_User_UpdatedBy",
                table: "Product",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItem_User_UpdatedBy",
                table: "ProductItem",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_User_UpdatedBy",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductItem_User_UpdatedBy",
                table: "ProductItem");

            migrationBuilder.DropIndex(
                name: "IX_ProductItem_UpdatedBy",
                table: "ProductItem");

            migrationBuilder.DropIndex(
                name: "IX_Product_UpdatedBy",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductItem");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ProductItem");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Product");
        }
    }
}
