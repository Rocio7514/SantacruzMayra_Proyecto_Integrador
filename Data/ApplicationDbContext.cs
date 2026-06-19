using Microsoft.EntityFrameworkCore;
using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Billetera> Billeteras => Set<Billetera>();
    public DbSet<Transaccion> Transacciones => Set<Transaccion>();
    public DbSet<Prediccion> Predicciones => Set<Prediccion>();
    public DbSet<BonoDiario> BonosDiarios => Set<BonoDiario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Billetera ---
        modelBuilder.Entity<Billetera>(entity =>
        {
            entity.ToTable("billeteras");
            entity.HasIndex(w => w.UsuarioId).IsUnique(); // un usuario = una billetera
            entity.Property(w => w.Saldo).HasColumnType("decimal(12,2)");
        });

        // --- Transaccion ---
        modelBuilder.Entity<Transaccion>(entity =>
        {
            entity.ToTable("transacciones");
            entity.Property(t => t.Monto).HasColumnType("decimal(12,2)");
            entity.Property(t => t.Tipo).HasConversion<string>().HasMaxLength(30);

            entity.HasOne(t => t.Billetera)
                  .WithMany(w => w.Transacciones)
                  .HasForeignKey(t => t.BilleteraId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Prediccion ---
        modelBuilder.Entity<Prediccion>(entity =>
        {
            entity.ToTable("predicciones");
            entity.Property(p => p.Monto).HasColumnType("decimal(12,2)");
            entity.Property(p => p.CuotaAplicada).HasColumnType("decimal(6,2)");
            entity.Property(p => p.ResultadoPronosticado).HasConversion<string>().HasMaxLength(10);
            entity.Property(p => p.Estado).HasConversion<string>().HasMaxLength(10);

            // Regla del proyecto: una sola predicción por usuario y partido
            entity.HasIndex(p => new { p.UsuarioId, p.PartidoId }).IsUnique();
        });

        // --- BonoDiario ---
        modelBuilder.Entity<BonoDiario>(entity =>
        {
            entity.ToTable("bonos_diarios");
            // Regla del proyecto: máximo un bono diario por usuario y día
            entity.HasIndex(d => new { d.UsuarioId, d.Fecha }).IsUnique();
        });
    }
}
