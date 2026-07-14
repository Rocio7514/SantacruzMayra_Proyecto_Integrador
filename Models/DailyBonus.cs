namespace UTNGolCoinApi.Models;

/// <summary>
/// Registra el día en que un usuario recibió su bono diario
/// (solo puede recibir uno por día, y solo si su saldo es 0).
/// </summary>
public class DailyBonus
{
    public int Id { get; set; }

    public int UserId { get; set; }

    /// <summary>Fecha (sin hora) en la que se otorgó el bono.</summary>
    public DateOnly Date { get; set; }
}
