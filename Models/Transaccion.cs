namespace UTNGolCoinApi.Models;

public enum TipoTransaccion
{
    BONO_BIENVENIDA,
    BONO_DIARIO,
    PREDICCION_DEBITO,
    PREMIO_CREDITO
}

public class Transaccion
{
    public int Id { get; set; }

    public int BilleteraId { get; set; }
    public Billetera? Billetera { get; set; }
    public TipoTransaccion Tipo { get; set; }
    public decimal Monto { get; set; }

    public string? Descripcion { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
