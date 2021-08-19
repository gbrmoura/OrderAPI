using Microsoft.EntityFrameworkCore;
using OrderAPI.Configs;
using OrderAPI.Enums;
using OrderAPI.Models;
using OrderAPI.Utils;
using System;

namespace OrderAPI.Database {
    public class DBContext : DbContext {

        public DBContext() { }

        public DBContext(DbContextOptions<DBContext> options) 
            : base(options) {
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //function to configure database
        }

        public DbSet<MUsuario> Usuario { get; set; }
        public DbSet<MUsuarioEndereco> UsuarioEndereco { get; set; }
        public DbSet<MUsuarioTelefone> UsuarioTelefone { get; set; }
        public DbSet<MFuncionario> Funcionario { get; set; }
        public DbSet<MCategoria> Categoria { get; set; }
        public DbSet<MPedido> Pedido { get; set; }
        public DbSet<MPedidoItem> PedidoItem { get; set; }
        public DbSet<MProduto> Produto { get; set; }
        public DbSet<MMetodoPagamento> MetodoPagamento { get; set; }
        public DbSet<MControleEstoque> ControleEstoque { get; set; }

    }
}
