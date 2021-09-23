using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderAPI.Migrations
{
    public partial class databasev111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produto_Categoria_CategoriaCodigo",
                table: "Produto");

            migrationBuilder.AlterColumn<int>(
                name: "CategoriaCodigo",
                table: "Produto",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_Categoria_CategoriaCodigo",
                table: "Produto",
                column: "CategoriaCodigo",
                principalTable: "Categoria",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produto_Categoria_CategoriaCodigo",
                table: "Produto");

            migrationBuilder.AlterColumn<int>(
                name: "CategoriaCodigo",
                table: "Produto",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_Categoria_CategoriaCodigo",
                table: "Produto",
                column: "CategoriaCodigo",
                principalTable: "Categoria",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
