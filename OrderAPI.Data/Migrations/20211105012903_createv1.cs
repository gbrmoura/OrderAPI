using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace OrderAPI.Data.Migrations
{
    public partial class createv1 : Migration
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
                    Descricao = table.Column<string>(type: "varchar(245)", maxLength: 245, nullable: true),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Funcionario",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(115)", maxLength: 115, nullable: false),
                    Login = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    Previlegio = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
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
                    Titulo = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodoPagamento", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Token",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Actor = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token", x => x.Codigo);
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
                    Token = table.Column<byte[]>(type: "varbinary(16)", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Codigo);
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
                    CategoriaCodigo = table.Column<int>(type: "int", nullable: false),
                    ImageCodigo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
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
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Data = table.Column<DateTime>(type: "datetime", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(245)", maxLength: 245, nullable: true),
                    UsuarioCodigo = table.Column<int>(type: "int", nullable: false),
                    MetodoPagamentoCodigo = table.Column<int>(type: "int", nullable: false),
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
                name: "ControleEstoque",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "varchar(245)", maxLength: 245, nullable: true),
                    Data = table.Column<DateTime>(type: "datetime", nullable: false),
                    ProdutoCodigo = table.Column<int>(type: "int", nullable: false),
                    FuncionarioCodigo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControleEstoque", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_ControleEstoque_Funcionario_FuncionarioCodigo",
                        column: x => x.FuncionarioCodigo,
                        principalTable: "Funcionario",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ControleEstoque_Produto_ProdutoCodigo",
                        column: x => x.ProdutoCodigo,
                        principalTable: "Produto",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    Caminho = table.Column<string>(type: "text", nullable: false),
                    ProductCodigo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Image_Produto_ProductCodigo",
                        column: x => x.ProductCodigo,
                        principalTable: "Produto",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoItem",
                columns: table => new
                {
                    Codigo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<float>(type: "float", nullable: false),
                    ProdutoCodigo = table.Column<int>(type: "int", nullable: false),
                    PedidoCodigo = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
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
                name: "IX_ControleEstoque_FuncionarioCodigo",
                table: "ControleEstoque",
                column: "FuncionarioCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_ControleEstoque_ProdutoCodigo",
                table: "ControleEstoque",
                column: "ProdutoCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Image_ProductCodigo",
                table: "Image",
                column: "ProductCodigo",
                unique: true);

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
                name: "Image");

            migrationBuilder.DropTable(
                name: "PedidoItem");

            migrationBuilder.DropTable(
                name: "Token");

            migrationBuilder.DropTable(
                name: "Funcionario");

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
