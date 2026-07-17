namespace UTNGolCoinApi.Models;

public enum ResultadoPronostico
{
    LOCAL,
    EMPATE,
    VISITANTE
}

public enum EstadoPrediccion
{
    PENDIENTE,
    GANADA,
    PERDIDA
}

public class Prediccion
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int PartidoId { get; set; }

    public ResultadoPronostico ResultadoPronosticado { get; set; }

    public decimal Monto { get; set; }

    public decimal CuotaAplicada { get; set; }

    public EstadoPrediccion Estado { get; set; } = EstadoPrediccion.PENDIENTE;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
