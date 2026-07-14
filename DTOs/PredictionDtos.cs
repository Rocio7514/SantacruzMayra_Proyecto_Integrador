namespace UTNGolCoinApi.DTOs;

public record CreatePredictionRequest(int UserId, int MatchId, string Prediction, decimal Amount);

public record PredictionResponse(
    int Id,
    int UserId,
    int MatchId,
    string PredictionType,
    decimal Amount,
    decimal Odds,
    string Status,
    DateTime CreatedAt);
