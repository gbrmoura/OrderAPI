using Microsoft.EntityFrameworkCore;
using OrderAPI.Models;

namespace OrderAPI.Database
{
    public class DBContext : DbContext {

        public DBContext() { }

        public DBContext(DbContextOptions<DBContext> options) 
            : base(options) {
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // TODO: Configura banco de dados!
        }

        public DbSet<MUsuario> Usuario { get; set; }
        public DbSet<MFuncionario> Funcionario { get; set; }
        public DbSet<MCategoria> Categoria { get; set; }
        public DbSet<MPedido> Pedido { get; set; }
        public DbSet<MPedidoItem> PedidoItem { get; set; }
        public DbSet<MProduto> Produto { get; set; }
        public DbSet<MMetodoPagamento> MetodoPagamento { get; set; }
        public DbSet<MControleEstoque> ControleEstoque { get; set; }

    }
}
