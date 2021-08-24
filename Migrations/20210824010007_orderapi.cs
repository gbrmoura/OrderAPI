using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace OrderAPI.Migrations
{
    public partial class orderapi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Titulo = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(245)", maxLength: 245, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "ControleEstoque",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
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
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(115)", maxLength: 115, nullable: false),
                    Login = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Senha = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    Previlegio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionario", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "MetodoPagamento",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodoPagamento", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioEndereco",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CEP = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: true),
                    Logradouro = table.Column<string>(type: "varchar(115)", maxLength: 115, nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Bairro = table.Column<string>(type: "varchar(145)", maxLength: 145, nullable: false),
                    Cidade = table.Column<string>(type: "varchar(115)", maxLength: 115, nullable: false),
                    Estado = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioEndereco", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Titulo = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(245)", maxLength: 245, nullable: true),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<float>(type: "float", nullable: false),
                    CategoriaCodigo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Produto_Categoria_CategoriaCodigo",
                        column: x => x.CategoriaCodigo,
                        principalTable: "Categoria",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Data = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(245)", maxLength: 245, nullable: true),
                    MetodoPagamentoCodigo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Pedido_MetodoPagamento_MetodoPagamentoCodigo",
                        column: x => x.MetodoPagamentoCodigo,
                        principalTable: "MetodoPagamento",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(115)", maxLength: 115, nullable: false),
                    Sobrenome = table.Column<string>(type: "varchar(145)", maxLength: 145, nullable: false),
                    Prontuario = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: true),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "varchar(245)", maxLength: 245, nullable: false),
                    EnderecoCodigo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Usuario_UsuarioEndereco_EnderecoCodigo",
                        column: x => x.EnderecoCodigo,
                        principalTable: "UsuarioEndereco",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItem",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<float>(type: "float", nullable: false),
                    ProdutoCodigo = table.Column<int>(type: "int", nullable: true),
                    PedidoCodigo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoItem", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_PedidoItem_Pedido_PedidoCodigo",
                        column: x => x.PedidoCodigo,
                        principalTable: "Pedido",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoItem_Produto_ProdutoCodigo",
                        column: x => x.ProdutoCodigo,
                        principalTable: "Produto",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioTelefone",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    DDD = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    Telefone = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    MUsuarioCodigo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioTelefone", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_UsuarioTelefone_Usuario_MUsuarioCodigo",
                        column: x => x.MUsuarioCodigo,
                        principalTable: "Usuario",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_MetodoPagamentoCodigo",
                table: "Pedido",
                column: "MetodoPagamentoCodigo");

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

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_EnderecoCodigo",
                table: "Usuario",
                column: "EnderecoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioTelefone_MUsuarioCodigo",
                table: "UsuarioTelefone",
                column: "MUsuarioCodigo");
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
                name: "UsuarioTelefone");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "MetodoPagamento");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "UsuarioEndereco");
        }
    }
}
