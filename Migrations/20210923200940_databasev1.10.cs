using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderAPI.Migrations
{
    public partial class databasev110 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoItem_Pedido_PedidoCodigo",
                table: "PedidoItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PedidoItem_Produto_ProdutoCodigo",
                table: "PedidoItem");

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoCodigo",
                table: "PedidoItem",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PedidoCodigo",
                table: "PedidoItem",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoItem_Pedido_PedidoCodigo",
                table: "PedidoItem",
                column: "PedidoCodigo",
                principalTable: "Pedido",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoItem_Produto_ProdutoCodigo",
                table: "PedidoItem",
                column: "ProdutoCodigo",
                principalTable: "Produto",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoItem_Pedido_PedidoCodigo",
                table: "PedidoItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PedidoItem_Produto_ProdutoCodigo",
                table: "PedidoItem");

            migrationBuilder.AlterColumn<int>(
                name: "ProdutoCodigo",
                table: "PedidoItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PedidoCodigo",
                table: "PedidoItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoItem_Pedido_PedidoCodigo",
                table: "PedidoItem",
                column: "PedidoCodigo",
                principalTable: "Pedido",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoItem_Produto_ProdutoCodigo",
                table: "PedidoItem",
                column: "ProdutoCodigo",
                principalTable: "Produto",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
