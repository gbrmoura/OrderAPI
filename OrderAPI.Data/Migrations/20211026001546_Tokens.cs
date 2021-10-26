using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderAPI.Data.Migrations
{
    public partial class Tokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Token",
                columns: table => new
                {
                    Codigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Actor = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token", x => x.Codigo);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Token");
        }
    }
}
