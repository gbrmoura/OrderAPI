using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderAPI.Migrations
{
    public partial class updatev1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "token",
                table: "Usuario",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "token",
                table: "Funcionario",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "token",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "token",
                table: "Funcionario");
        }
    }
}
