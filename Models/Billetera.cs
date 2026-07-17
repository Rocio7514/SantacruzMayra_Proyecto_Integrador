namespace UTNGolCoinApi.Models;

/// <summary>
/// Representa la billetera de UTNGolCoin de un usuario.
/// Cada usuario tiene exactamente una billetera (relación 1 a 1 lógica
/// con el usuario que vive en el Servicio de Estadísticas).
/// </summary>
public class Billetera
{
    public int Id { get; set; }

    /// <summary>Id del usuario, referenciado lógicamente desde el Servicio de Estadísticas.</summary>
    public int UsuarioId { get; set; }

    /// <summary>Saldo actual en UTNGolCoin.</summary>
    public decimal Saldo { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Navegación
    public ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
}
