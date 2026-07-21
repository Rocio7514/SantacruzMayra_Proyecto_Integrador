namespace UTNGolCoinApi.Models;

public class Billetera
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public decimal Saldo { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
}
