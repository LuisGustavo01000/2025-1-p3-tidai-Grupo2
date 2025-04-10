using Microsoft.EntityFrameworkCore;
using YourProject.Models;

namespace YourProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Conteudo> Conteudos { get; set; }
        public DbSet<Dashboard> Dashboards { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<MetaFinanceira> MetasFinanceiras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pessoa>()
                .HasMany(p => p.Transacoes)
                .WithOne(t => t.Usuario)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Dashboard>()
                .HasOne(d => d.Usuario)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MetaFinanceira>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}