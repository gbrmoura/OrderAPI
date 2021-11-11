using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace OrderAPI.Data.Migrations
{
    public partial class ProdutoFavorito : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Favorito",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ProdutoCodigo = table.Column<int>(type: "int", nullable: false),
                    UsuarioCodigo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorito", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Favorito_Produto_ProdutoCodigo",
                        column: x => x.ProdutoCodigo,
                        principalTable: "Produto",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorito_Usuario_UsuarioCodigo",
                        column: x => x.UsuarioCodigo,
                        principalTable: "Usuario",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favorito_ProdutoCodigo",
                table: "Favorito",
                column: "ProdutoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Favorito_UsuarioCodigo",
                table: "Favorito",
                column: "UsuarioCodigo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorito");
        }
    }
}
