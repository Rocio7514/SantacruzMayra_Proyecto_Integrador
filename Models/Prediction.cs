namespace UTNGolCoinApi.Models;

/// <summary>Resultado 1X2 pronosticado por el usuario.</summary>
public enum PredictionType
{
    Local,   // "1"
    Draw,    // "X"
    Visitor  // "2"
}

/// <summary>Estado de liquidación de la predicción.</summary>
public enum PredictionStatus
{
    Pending,
    Won,
    Lost
}

/// <summary>
/// Una predicción/apuesta de un usuario sobre un partido específico.
/// Restricción: un usuario solo puede tener UNA predicción por partido
/// (se aplica con un índice único en la base de datos, ver ApplicationDbContext).
/// </summary>
public class Prediction
{
    public int Id { get; set; }

    public int UserId { get; set; }

    /// <summary>Id del partido, referenciado lógicamente desde el Servicio de Estadísticas.</summary>
    public int MatchId { get; set; }

    public PredictionType PredictionType { get; set; }

    /// <summary>Monto apostado en UTNGolCoin.</summary>
    public decimal Amount { get; set; }

    /// <summary>Cuota vigente al momento de crear la predicción.</summary>
    public decimal Odds { get; set; }

    public PredictionStatus Status { get; set; } = PredictionStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
