using EfTestApp.Model.Tables;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace EfTestApp.Model
{
    public partial class TestDbContext : DbContext
    {
        public TestDbContext()
        {
        }

        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Musteri> Musteri { get; set; }
        public virtual DbSet<Siparis> Siparis { get; set; }
        public virtual DbSet<Urun> Urun { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("data source=(localdb)\\MsSqlLocalDb;initial catalog=testdb;Integrated Security=SSPI;persist security info=true;attachdbfilename=C:\\projtemp\\EfTestApp\\EfTestApp\\EfTestApp\\testdb.mdf");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Musteri>(entity =>
            {
                entity.Property(e => e.AdSoyad)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Adres).HasMaxLength(500);
            });

            modelBuilder.Entity<Siparis>(entity =>
            {
                entity.Property(e => e.Miktar).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.SiparisTarihi).HasColumnType("datetime");

                entity.HasOne(d => d.Musteri)
                    .WithMany(p => p.Siparis)
                    .HasForeignKey(d => d.MusteriId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Siparis__Musteri__29572725");

                entity.HasOne(d => d.Urun)
                    .WithMany(p => p.Siparis)
                    .HasForeignKey(d => d.UrunId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Siparis__UrunId__286302EC");
            });

            modelBuilder.Entity<Urun>(entity =>
            {
                entity.Property(e => e.UrunAdi)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
