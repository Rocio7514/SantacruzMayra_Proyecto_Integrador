namespace UTNGolCoinApi.Models;

/// <summary>
/// Tipos de movimiento posibles en el ledger. Nunca se borran ni editan
/// transacciones existentes: el ledger es un historial append-only.
/// </summary>
public enum TransactionType
{
    WelcomeBonus,
    Prediction,
    Prize,
    DailyBonus
}

/// <summary>
/// Registro inmutable de un movimiento de saldo. Esta es la tabla más
/// importante del servicio: es la fuente de verdad de todo movimiento
/// de UTNGolCoin.
/// </summary>
public class Transaction
{
    public int Id { get; set; }

    public int WalletId { get; set; }
    public Wallet? Wallet { get; set; }

    public TransactionType Type { get; set; }

    /// <summary>Monto con signo: positivo = ingreso, negativo = egreso.</summary>
    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;
}
