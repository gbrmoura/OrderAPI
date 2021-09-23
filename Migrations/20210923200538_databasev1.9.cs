using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderAPI.Migrations
{
    public partial class databasev19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_MetodoPagamento_MetodoPagamentoCodigo",
                table: "Pedido");

            migrationBuilder.AlterColumn<int>(
                name: "MetodoPagamentoCodigo",
                table: "Pedido",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_MetodoPagamento_MetodoPagamentoCodigo",
                table: "Pedido",
                column: "MetodoPagamentoCodigo",
                principalTable: "MetodoPagamento",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_MetodoPagamento_MetodoPagamentoCodigo",
                table: "Pedido");

            migrationBuilder.AlterColumn<int>(
                name: "MetodoPagamentoCodigo",
                table: "Pedido",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_MetodoPagamento_MetodoPagamentoCodigo",
                table: "Pedido",
                column: "MetodoPagamentoCodigo",
                principalTable: "MetodoPagamento",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
