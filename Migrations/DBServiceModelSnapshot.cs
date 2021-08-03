﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderAPI.Database;

namespace OrderAPI.Migrations
{
    [DbContext(typeof(DBService))]
    partial class DBServiceModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.8");

            modelBuilder.Entity("OrderAPI.Models.MUsuario", b =>
                {
                    b.Property<int>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("DataCadastro")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("Current_TimeStamp()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(245)
                        .HasColumnType("varchar(245)");

                    b.Property<int>("NivelAcesso")
                        .HasColumnType("int");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Sobrenome")
                        .IsRequired()
                        .HasMaxLength(110)
                        .HasColumnType("varchar(110)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<byte[]>("Token")
                        .IsRequired()
                        .HasColumnType("varbinary(16)");

                    b.HasKey("Codigo");

                    b.ToTable("Usuario");
                });
#pragma warning restore 612, 618
        }
    }
}
