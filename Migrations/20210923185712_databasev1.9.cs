using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderAPI.Migrations
{
    public partial class databasev19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Usuario",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "PedidoItem",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PedidoItem");

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
        }
    }
}
