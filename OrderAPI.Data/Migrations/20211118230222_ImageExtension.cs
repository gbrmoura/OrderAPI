using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderAPI.Data.Migrations
{
    public partial class ImageExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Extensao",
                table: "Image",
                type: "varchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extensao",
                table: "Image");
        }
    }
}
