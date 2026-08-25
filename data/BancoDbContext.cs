using Microsoft.EntityFrameworkCore;
using Sistema_banc_rio_falso.Models;

namespace Sistema_banc_rio_falso.Data
{
    public class BancoDbContext : DbContext
    {
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Administrador> Administradores { get; set; } // Nova entidade admin

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Cria um arquivo local chamado 'banco_bancofalso.db' na máquina
            optionsBuilder.UseSqlite("Data Source=banco_bancofalso.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Regra de Negócio: O CPF da conta deve ser único no banco de dados!
            modelBuilder.Entity<Conta>()
                .HasIndex(c => c.Cpf)
                .IsUnique();
        }
    }
}