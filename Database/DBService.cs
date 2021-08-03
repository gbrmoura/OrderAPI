using Microsoft.EntityFrameworkCore;
using OrderAPI.Configs;
using OrderAPI.Models;
using OrderAPI.Utils;
using System;

namespace OrderAPI.Database {
    public class DBService : DbContext {

        public DBService() { }

        public DBService(DbContextOptions<DBService> options) : base(options) {
        
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            DBConfig config = new DBConfig();
            String connection = SystemUtils.Log(config.ConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<MUsuario>()
                .Property(field => field.DataCadastro)
                .HasDefaultValueSql("Current_TimeStamp()");
        }

        public DbSet<MUsuario> Usuario { get; set; }

    }
}
