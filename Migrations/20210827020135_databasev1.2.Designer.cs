﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderAPI.Database;

namespace OrderAPI.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20210827020135_databasev1.2")]
    partial class databasev12
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.9");

            modelBuilder.Entity("OrderAPI.Models.MCategoria", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Descricao")
                        .HasMaxLength(245)
                        .HasColumnType("varchar(245)");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("varchar(45)");

                    b.HasKey("Codigo");

                    b.ToTable("Categoria");
                });

            modelBuilder.Entity("OrderAPI.Models.MControleEstoque", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Quantidade")
                        .HasColumnType("int");

                    b.Property<string>("observacao")
                        .HasMaxLength(245)
                        .HasColumnType("varchar(245)");

                    b.HasKey("Codigo");

                    b.ToTable("ControleEstoque");
                });

            modelBuilder.Entity("OrderAPI.Models.MFuncionario", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(115)
                        .HasColumnType("varchar(115)");

                    b.Property<int>("Previlegio")
                        .HasColumnType("int");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.HasKey("Codigo");

                    b.ToTable("Funcionario");
                });

            modelBuilder.Entity("OrderAPI.Models.MMetodoPagamento", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("varchar(45)");

                    b.HasKey("Codigo");

                    b.ToTable("MetodoPagamento");
                });

            modelBuilder.Entity("OrderAPI.Models.MPedido", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Data")
                        .HasColumnType("datetime");

                    b.Property<int?>("MetodoPagamentoCodigo")
                        .HasColumnType("int");

                    b.Property<string>("Observacao")
                        .HasMaxLength(245)
                        .HasColumnType("varchar(245)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Codigo");

                    b.HasIndex("MetodoPagamentoCodigo");

                    b.ToTable("Pedido");
                });

            modelBuilder.Entity("OrderAPI.Models.MPedidoItem", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("PedidoCodigo")
                        .HasColumnType("int");

                    b.Property<int?>("ProdutoCodigo")
                        .HasColumnType("int");

                    b.Property<int>("Quantidade")
                        .HasColumnType("int");

                    b.Property<float>("Valor")
                        .HasColumnType("float");

                    b.HasKey("Codigo");

                    b.HasIndex("PedidoCodigo");

                    b.HasIndex("ProdutoCodigo");

                    b.ToTable("PedidoItem");
                });

            modelBuilder.Entity("OrderAPI.Models.MProduto", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("CategoriaCodigo")
                        .HasColumnType("int");

                    b.Property<string>("Descricao")
                        .HasMaxLength(245)
                        .HasColumnType("varchar(245)");

                    b.Property<int>("Quantidade")
                        .HasColumnType("int");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("varchar(45)");

                    b.Property<float>("Valor")
                        .HasColumnType("float");

                    b.HasKey("Codigo");

                    b.HasIndex("CategoriaCodigo");

                    b.ToTable("Produto");
                });

            modelBuilder.Entity("OrderAPI.Models.MUsuario", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(245)
                        .HasColumnType("varchar(245)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(115)
                        .HasColumnType("varchar(115)");

                    b.Property<string>("Prontuario")
                        .HasMaxLength(14)
                        .HasColumnType("varchar(14)");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Sobrenome")
                        .IsRequired()
                        .HasMaxLength(145)
                        .HasColumnType("varchar(145)");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.HasKey("Codigo");

                    b.ToTable("Usuario");
                });

            modelBuilder.Entity("OrderAPI.Models.MPedido", b =>
                {
                    b.HasOne("OrderAPI.Models.MMetodoPagamento", "MetodoPagamento")
                        .WithMany()
                        .HasForeignKey("MetodoPagamentoCodigo");

                    b.Navigation("MetodoPagamento");
                });

            modelBuilder.Entity("OrderAPI.Models.MPedidoItem", b =>
                {
                    b.HasOne("OrderAPI.Models.MPedido", "Pedido")
                        .WithMany("Items")
                        .HasForeignKey("PedidoCodigo");

                    b.HasOne("OrderAPI.Models.MProduto", "Produto")
                        .WithMany()
                        .HasForeignKey("ProdutoCodigo");

                    b.Navigation("Pedido");

                    b.Navigation("Produto");
                });

            modelBuilder.Entity("OrderAPI.Models.MProduto", b =>
                {
                    b.HasOne("OrderAPI.Models.MCategoria", "Categoria")
                        .WithMany("Produtos")
                        .HasForeignKey("CategoriaCodigo");

                    b.Navigation("Categoria");
                });

            modelBuilder.Entity("OrderAPI.Models.MCategoria", b =>
                {
                    b.Navigation("Produtos");
                });

            modelBuilder.Entity("OrderAPI.Models.MPedido", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}