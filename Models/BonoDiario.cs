namespace UTNGolCoinApi.Models;

/// <summary>
/// Registra el día en que un usuario recibió su bono diario
/// (solo puede recibir uno por día, y solo si su saldo es 0).
/// </summary>
public class BonoDiario
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    /// <summary>Fecha (sin hora) en la que se otorgó el bono.</summary>
    public DateOnly Fecha { get; set; }
}
