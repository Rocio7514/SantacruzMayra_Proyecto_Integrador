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
    public DbSet<ConfiguracionSistema> Configuracion => Set<ConfiguracionSistema>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Billetera>(entity =>
        {
            entity.ToTable("billeteras");
            entity.HasIndex(w => w.UsuarioId).IsUnique(); 
            entity.Property(w => w.Saldo).HasColumnType("decimal(12,2)");
        });

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


        modelBuilder.Entity<Prediccion>(entity =>
        {
            entity.ToTable("predicciones");
            entity.Property(p => p.Monto).HasColumnType("decimal(12,2)");
            entity.Property(p => p.CuotaAplicada).HasColumnType("decimal(6,2)");
            entity.Property(p => p.ResultadoPronosticado).HasConversion<string>().HasMaxLength(10);
            entity.Property(p => p.Estado).HasConversion<string>().HasMaxLength(10);

            entity.HasIndex(p => new { p.UsuarioId, p.PartidoId }).IsUnique();
        });

        modelBuilder.Entity<BonoDiario>(entity =>
        {
            entity.ToTable("bonos_diarios");
           
            entity.HasIndex(d => new { d.UsuarioId, d.Fecha }).IsUnique();
        });

        modelBuilder.Entity<ConfiguracionSistema>(entity =>
        {
            entity.ToTable("configuracion");
            entity.Property(c => c.BonoInicial).HasColumnType("decimal(12,2)");
            entity.Property(c => c.MonedasPorAcierto).HasColumnType("decimal(12,2)");
            entity.Property(c => c.LimiteMaximoApuesta).HasColumnType("decimal(12,2)");
        });
    }
}
