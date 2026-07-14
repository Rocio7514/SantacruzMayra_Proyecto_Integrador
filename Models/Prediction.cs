namespace UTNGolCoinApi.Models;
public enum PredictionType
{
    Local,   // "1"
    Draw,    // "X"
    Visitor  // "2"
}
public enum PredictionStatus
{
    Pending,
    Won,
    Lost
}

public class Prediction
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public int MatchId { get; set; }
   public PredictionType PredictionType { get; set; }
    public decimal Amount { get; set; }
    public decimal Odds { get; set; }
    public PredictionStatus Status { get; set; } = PredictionStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
