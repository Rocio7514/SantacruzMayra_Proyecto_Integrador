namespace UTNGolCoinApi.DTOs;

/// <summary>Body para POST /prediction.</summary>
public record CreatePredictionRequest(int UserId, int MatchId, string Prediction, decimal Amount);

/// <summary>Respuesta al crear/consultar una predicción.</summary>
public record PredictionResponse(
    int Id,
    int UserId,
    int MatchId,
    string PredictionType,
    decimal Amount,
    decimal Odds,
    string Status,
    DateTime CreatedAt);
