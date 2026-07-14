using Microsoft.EntityFrameworkCore;
using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Prediction> Predictions => Set<Prediction>();
    public DbSet<DailyBonus> DailyBonuses => Set<DailyBonus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Wallet
        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasIndex(w => w.UserId).IsUnique(); // un usuario = una billetera
            entity.Property(w => w.Balance).HasColumnType("decimal(12,2)");
        });

        // Transaction
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(t => t.Amount).HasColumnType("decimal(12,2)");
            entity.Property(t => t.Type).HasConversion<string>().HasMaxLength(30);

            entity.HasOne(t => t.Wallet)
                  .WithMany(w => w.Transactions)
                  .HasForeignKey(t => t.WalletId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Prediction 
        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.Property(p => p.Amount).HasColumnType("decimal(12,2)");
            entity.Property(p => p.Odds).HasColumnType("decimal(6,2)");
            entity.Property(p => p.PredictionType).HasConversion<string>().HasMaxLength(10);
            entity.Property(p => p.Status).HasConversion<string>().HasMaxLength(10);

            // Una sola predicción por usuario y partido
            entity.HasIndex(p => new { p.UserId, p.MatchId }).IsUnique();
        });

        // DailyBonus
        modelBuilder.Entity<DailyBonus>(entity =>
        {
            // Regla del proyecto: máximo un bono diario por usuario y día
            entity.HasIndex(d => new { d.UserId, d.Date }).IsUnique();
        });
    }
}
