using CashWiseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CashWiseAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pessoa> Pessoas { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Conteudo> Conteudos { get; set; } = null!;
        public DbSet<Dashboard> Dashboards { get; set; } = null!;
        public DbSet<Transacao> Transacoes { get; set; } = null!;
        public DbSet<MetaFinanceira> MetasFinanceiras { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pessoa>()
                .HasMany(p => p.Transacoes)
                .WithOne(t => t.Usuario)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Dashboard>()
                .HasOne(d => d.Usuario)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MetaFinanceira>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
