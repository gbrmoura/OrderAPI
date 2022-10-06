using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderAPI.Data.Migrations
{
    public partial class ControleEstoque : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "ControleEstoque",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "ControleEstoque");
        }
    }
}
