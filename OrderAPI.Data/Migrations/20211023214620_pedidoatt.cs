using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderAPI.Data.Migrations
{
    public partial class pedidoatt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Pedido",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Observacao",
                table: "Pedido",
                type: "varchar(245)",
                maxLength: 245,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(245)",
                oldMaxLength: 245);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCodigo",
                table: "Pedido",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_UsuarioCodigo",
                table: "Pedido",
                column: "UsuarioCodigo");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_Usuario_UsuarioCodigo",
                table: "Pedido",
                column: "UsuarioCodigo",
                principalTable: "Usuario",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_Usuario_UsuarioCodigo",
                table: "Pedido");

            migrationBuilder.DropIndex(
                name: "IX_Pedido_UsuarioCodigo",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "UsuarioCodigo",
                table: "Pedido");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Pedido",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Observacao",
                table: "Pedido",
                type: "varchar(245)",
                maxLength: 245,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(245)",
                oldMaxLength: 245,
                oldNullable: true);
        }
    }
}
