using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Models;
using UTNGolCoinApi.Repositories;

namespace UTNGolCoinApi.Services;

/// <summary>
/// Corazón del proyecto (RF12/RF19): liquida automáticamente todas las
/// predicciones pendientes de un partido cuando el Servicio de Estadísticas
/// notifica el resultado oficial.
/// </summary>
public interface IRewardService
{
    Task<ProcessRewardsResponse> ProcessRewardsAsync(ProcessRewardsRequest request);
}

public class RewardService : IRewardService
{
    private readonly IPredictionRepository _predictionRepository;
    private readonly IWalletRepository _walletRepository;

    public RewardService(IPredictionRepository predictionRepository, IWalletRepository walletRepository)
    {
        _predictionRepository = predictionRepository;
        _walletRepository = walletRepository;
    }

    public async Task<ProcessRewardsResponse> ProcessRewardsAsync(ProcessRewardsRequest request)
    {
        var winnerType = ParsePredictionType(request.Winner);
        var pendientes = await _predictionRepository.GetPendingByMatchIdAsync(request.MatchId);

        int ganadores = 0;
        decimal totalPagado = 0m;

        foreach (var prediction in pendientes)
        {
            if (prediction.PredictionType == winnerType)
            {
                var premio = prediction.Amount * prediction.Odds;
                prediction.Status = PredictionStatus.Won;

                var wallet = await _walletRepository.GetByUserIdAsync(prediction.UserId);
                if (wallet is null) continue; // no debería pasar, pero no rompemos la liquidación completa

                wallet.Balance += premio;
                wallet.Transactions.Add(new Transaction
                {
                    Type = TransactionType.Prize,
                    Amount = premio,
                    Description = $"Premio partido #{request.MatchId}"
                });

                ganadores++;
                totalPagado += premio;
            }
            else
            {
                prediction.Status = PredictionStatus.Lost;
            }
        }

        await _predictionRepository.SaveChangesAsync();

        return new ProcessRewardsResponse(request.MatchId, pendientes.Count, ganadores, totalPagado);
    }

    private static PredictionType ParsePredictionType(string value) => value switch
    {
        "1" => PredictionType.Local,
        "X" or "x" => PredictionType.Draw,
        "2" => PredictionType.Visitor,
        _ => throw new InvalidPredictionValueException(value)
    };
}
