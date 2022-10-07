using Microsoft.EntityFrameworkCore;
using OrderAPI.Domain.Models;

namespace OrderAPI.Data
{
    public class OrderAPIContext : DbContext 
    {
        public OrderAPIContext(DbContextOptions<OrderAPIContext> options) : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<CategoriaModel>()
                .Property(p => p.Status)
                .HasDefaultValue(1);

            modelBuilder.Entity<FuncionarioModel>()
                .Property(p => p.Status)
                .HasDefaultValue(1);

            modelBuilder.Entity<MetodoPagamentoModel>()
                .Property(p => p.Status)
                .HasDefaultValue(1);

            modelBuilder.Entity<PedidoItemModel>()
                .Property(p => p.Status)
                .HasDefaultValue(1);
            
            modelBuilder.Entity<ProdutoModel>()
                .Property(p => p.Status)
                .HasDefaultValue(1);

            modelBuilder.Entity<UsuarioModel>()
                .Property(p => p.Status)
                .HasDefaultValue(1);
            
            modelBuilder.Entity<ControleEstoqueModel>()
                .Property(p => p.Status)
                .HasDefaultValue(1);

            modelBuilder.Entity<ImageModel>()
                .Property(p => p.Status)
                .HasDefaultValue(1);
            
            modelBuilder.Entity<FavoritoModel>()
                .Property(p => p.Status)
                .HasDefaultValue(1);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UsuarioModel> Usuario { get; set; }
        public DbSet<FuncionarioModel> Funcionario { get; set; }
        public DbSet<CategoriaModel> Categoria { get; set; }
        public DbSet<PedidoModel> Pedido { get; set; }
        public DbSet<PedidoItemModel> PedidoItem { get; set; }
        public DbSet<ProdutoModel> Produto { get; set; }
        public DbSet<MetodoPagamentoModel> MetodoPagamento { get; set; }
        public DbSet<ControleEstoqueModel> ControleEstoque { get; set; }
        public DbSet<ImageModel> Image { get; set; }
        public DbSet<TokenModel> Token { get; set; }
        public DbSet<FavoritoModel> Favorito { get; set; }
        public DbSet<RecoverPasswordModel> RecoverPassword { get; set; }

    }
}
