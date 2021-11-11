using Microsoft.EntityFrameworkCore;
using OrderAPI.Data.Models;

namespace OrderAPI.Data
{
    public class OrderAPIContext : DbContext 
    {
        public OrderAPIContext(DbContextOptions<OrderAPIContext> options) : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<MCategoria>()
                .Property(p => p.Status)
                .HasDefaultValue(1);

            modelBuilder.Entity<MFuncionario>()
                .Property(p => p.Status)
                .HasDefaultValue(1);

            modelBuilder.Entity<MMetodoPagamento>()
                .Property(p => p.Status)
                .HasDefaultValue(1);

            modelBuilder.Entity<MPedidoItem>()
                .Property(p => p.Status)
                .HasDefaultValue(1);
            
            modelBuilder.Entity<MProduto>()
                .Property(p => p.Status)
                .HasDefaultValue(1);

            modelBuilder.Entity<MUsuario>()
                .Property(p => p.Status)
                .HasDefaultValue(1);
            
            modelBuilder.Entity<MControleEstoque>()
                .Property(p => p.Status)
                .HasDefaultValue(1);

            modelBuilder.Entity<MImage>()
                .Property(p => p.Status)
                .HasDefaultValue(1);
            
            modelBuilder.Entity<MFavorito>()
                .Property(p => p.Status)
                .HasDefaultValue(1);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<MUsuario> Usuario { get; set; }
        public DbSet<MFuncionario> Funcionario { get; set; }
        public DbSet<MCategoria> Categoria { get; set; }
        public DbSet<MPedido> Pedido { get; set; }
        public DbSet<MPedidoItem> PedidoItem { get; set; }
        public DbSet<MProduto> Produto { get; set; }
        public DbSet<MMetodoPagamento> MetodoPagamento { get; set; }
        public DbSet<MControleEstoque> ControleEstoque { get; set; }
        public DbSet<MImage> Image { get; set; }
        public DbSet<MToken> Token { get; set; }
        public DbSet<MFavorito> Favorito { get; set; }

    }
}
