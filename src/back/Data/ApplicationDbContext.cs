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

        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Conteudo> Conteudos { get; set; } = null!;
        public DbSet<Dashboard> Dashboards { get; set; } = null!;
        public DbSet<Transacao> Transacoes { get; set; } = null!;
        public DbSet<MetaFinanceira> MetasFinanceiras { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.DataCriacao).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                entity.HasMany(u => u.Transacoes)
                    .WithOne(t => t.Usuario)
                    .HasForeignKey("UsuarioId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Transacao>(entity =>
            {
                entity.ToTable("TRANSACAO");
                entity.Property(t => t.Id).HasColumnName("ID_TRANSACAO");
                entity.Property(t => t.Descricao).HasColumnName("DESCRICAO_CONT");
                entity.Property(t => t.Valor).HasColumnName("VALOR_TRANS");
                entity.Property(t => t.Tipo).HasColumnName("TIPO_TRANS").HasMaxLength(20);
                entity.Property(t => t.Data).HasColumnName("DATA_TRANS").HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
                entity.Property<int>("UsuarioId").HasColumnName("USUARIOFK");
            });

            modelBuilder.Entity<Dashboard>(entity =>
            {
                entity.ToTable("DASHBOARD");
                entity.Property(d => d.Id).HasColumnName("ID_DASH");
                entity.Property(d => d.SaldoTotal).HasColumnName("SALDOTOTAL_DASH");
                entity.Property(d => d.InvestimentoTotal).HasColumnName("INVESTIMENTOTOTAIS_DASH");
                entity.Property<int>("UsuarioId").HasColumnName("USUARIOFK");

                entity.HasOne(d => d.Usuario)
                    .WithMany()
                    .HasForeignKey("UsuarioId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MetaFinanceira>(entity =>
            {
                entity.ToTable("META_FINANCEIRA");
                entity.Property(m => m.Id).HasColumnName("ID_META");
                entity.Property(m => m.Nome).HasColumnName("NOME_META").HasMaxLength(56);
                entity.Property(m => m.Valor).HasColumnName("VALOR_META");
                entity.Property(m => m.Prazo).HasColumnName("PRAZO_META");
                entity.Property(m => m.Status).HasColumnName("STATUS_META").HasMaxLength(20);
                entity.Property<int>("UsuarioId").HasColumnName("USUARIOFK");

                entity.HasOne(m => m.Usuario)
                    .WithMany()
                    .HasForeignKey("UsuarioId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
