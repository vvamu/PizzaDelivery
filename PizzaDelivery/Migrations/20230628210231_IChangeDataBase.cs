using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaDelivery.Migrations
{
    /// <inheritdoc />
    public partial class IChangeDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Promocodes_PromocodeId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ShoppingCart_ShoppingCartId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PromocodeId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShoppingCartId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShoppindCardId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShoppingCartId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "OrderTotal",
                table: "Orders",
                newName: "TotalPrice");

            migrationBuilder.AddColumn<Guid>(
                name: "PromocodeId",
                table: "ShoppingCart",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "ShoppingCart",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ShoppingCart",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "ShoopingCartPizzas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PromocodeId",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ShoppindCardId",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_PromocodeId",
                table: "ShoppingCart",
                column: "PromocodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_UserId",
                table: "ShoppingCart",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoopingCartPizzas_OrderId",
                table: "ShoopingCartPizzas",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoopingCartPizzas_Orders_OrderId",
                table: "ShoopingCartPizzas",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_AspNetUsers_UserId",
                table: "ShoppingCart",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_Promocodes_PromocodeId",
                table: "ShoppingCart",
                column: "PromocodeId",
                principalTable: "Promocodes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoopingCartPizzas_Orders_OrderId",
                table: "ShoopingCartPizzas");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_AspNetUsers_UserId",
                table: "ShoppingCart");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_Promocodes_PromocodeId",
                table: "ShoppingCart");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCart_PromocodeId",
                table: "ShoppingCart");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCart_UserId",
                table: "ShoppingCart");

            migrationBuilder.DropIndex(
                name: "IX_ShoopingCartPizzas_OrderId",
                table: "ShoopingCartPizzas");

            migrationBuilder.DropColumn(
                name: "PromocodeId",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ShoopingCartPizzas");

            migrationBuilder.DropColumn(
                name: "ShoppindCardId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Orders",
                newName: "OrderTotal");

            migrationBuilder.AlterColumn<Guid>(
                name: "PromocodeId",
                table: "Orders",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Orders",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<Guid>(
                name: "ShoppindCardId",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ShoppingCartId",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PromocodeId",
                table: "Orders",
                column: "PromocodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShoppingCartId",
                table: "Orders",
                column: "ShoppingCartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Promocodes_PromocodeId",
                table: "Orders",
                column: "PromocodeId",
                principalTable: "Promocodes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ShoppingCart_ShoppingCartId",
                table: "Orders",
                column: "ShoppingCartId",
                principalTable: "ShoppingCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
