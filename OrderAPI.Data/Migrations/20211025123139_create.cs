using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderAPI.Data.Migrations
{
    public partial class create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Codigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(245)", maxLength: 245, nullable: true),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "ControleEstoque",
                columns: table => new
                {
                    Codigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    observacao = table.Column<string>(type: "varchar(245)", maxLength: 245, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControleEstoque", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Funcionario",
                columns: table => new
                {
                    Codigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Nome = table.Column<string>(type: "varchar(115)", maxLength: 115, nullable: false),
                    Login = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    Previlegio = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionario", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "MetodoPagamento",
                columns: table => new
                {
                    Codigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Nome = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodoPagamento", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Codigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Nome = table.Column<string>(type: "varchar(115)", maxLength: 115, nullable: false),
                    Sobrenome = table.Column<string>(type: "varchar(145)", maxLength: 145, nullable: false),
                    Prontuario = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: true),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "varchar(245)", maxLength: 245, nullable: false),
                    Token = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Codigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Titulo = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(245)", maxLength: 245, nullable: true),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<float>(type: "float", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CategoriaCodigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Produto_Categoria_CategoriaCodigo",
                        column: x => x.CategoriaCodigo,
                        principalTable: "Categoria",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    Codigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(245)", maxLength: 245, nullable: true),
                    UsuarioCodigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    MetodoPagamentoCodigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Pedido_MetodoPagamento_MetodoPagamentoCodigo",
                        column: x => x.MetodoPagamentoCodigo,
                        principalTable: "MetodoPagamento",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pedido_Usuario_UsuarioCodigo",
                        column: x => x.UsuarioCodigo,
                        principalTable: "Usuario",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItem",
                columns: table => new
                {
                    Codigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<float>(type: "float", nullable: false),
                    ProdutoCodigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    PedidoCodigo = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItem", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_PedidoItem_Pedido_PedidoCodigo",
                        column: x => x.PedidoCodigo,
                        principalTable: "Pedido",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoItem_Produto_ProdutoCodigo",
                        column: x => x.ProdutoCodigo,
                        principalTable: "Produto",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_MetodoPagamentoCodigo",
                table: "Pedido",
                column: "MetodoPagamentoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_UsuarioCodigo",
                table: "Pedido",
                column: "UsuarioCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItem_PedidoCodigo",
                table: "PedidoItem",
                column: "PedidoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoItem_ProdutoCodigo",
                table: "PedidoItem",
                column: "ProdutoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_CategoriaCodigo",
                table: "Produto",
                column: "CategoriaCodigo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControleEstoque");

            migrationBuilder.DropTable(
                name: "Funcionario");

            migrationBuilder.DropTable(
                name: "PedidoItem");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "MetodoPagamento");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Categoria");
        }
    }
}
