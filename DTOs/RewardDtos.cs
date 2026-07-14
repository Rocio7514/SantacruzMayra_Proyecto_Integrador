namespace UTNGolCoinApi.DTOs;

public record ProcessRewardsRequest(int MatchId, string Winner);
public record ProcessRewardsResponse(int MatchId, int PredictionsEvaluated, int Winners, decimal TotalPaid);
public record DailyBonusResponse(bool Granted, string Message, decimal NewBalance);
