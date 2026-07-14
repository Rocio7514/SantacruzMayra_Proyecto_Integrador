namespace UTNGolCoinApi.DTOs;

/// <summary>
/// Body para POST /api/rewards/process — lo envía el Servicio de Estadísticas
/// cuando el administrador registra el resultado oficial de un partido.
/// "winner" usa la misma convención 1/X/2 que las predicciones.
/// </summary>
public record ProcessRewardsRequest(int MatchId, string Winner);

/// <summary>Resumen devuelto tras liquidar un partido.</summary>
public record ProcessRewardsResponse(int MatchId, int PredictionsEvaluated, int Winners, decimal TotalPaid);

/// <summary>Respuesta del endpoint de bono diario.</summary>
public record DailyBonusResponse(bool Granted, string Message, decimal NewBalance);
