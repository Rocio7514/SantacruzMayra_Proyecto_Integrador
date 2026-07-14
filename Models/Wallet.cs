namespace UTNGolCoinApi.Models;

/// <summary>
/// Representa la billetera de UTNGolCoin de un usuario.
/// Cada usuario tiene exactamente una billetera (relación 1 a 1 lógica
/// con el usuario que vive en el Servicio de Estadísticas).
/// </summary>
public class Wallet
{
    public int Id { get; set; }

    /// <summary>Id del usuario, referenciado lógicamente desde el Servicio de Estadísticas.</summary>
    public int UserId { get; set; }

    /// <summary>Saldo actual en UTNGolCoin.</summary>
    public decimal Balance { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
