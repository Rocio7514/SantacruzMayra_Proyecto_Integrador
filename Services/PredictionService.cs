using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Models;
using UTNGolCoinApi.Repositories;

namespace UTNGolCoinApi.Services;

public interface IPredictionService
{
    Task<PredictionResponse> CreatePredictionAsync(CreatePredictionRequest request);
    Task<List<PredictionResponse>> GetByUserIdAsync(int userId);
}

public class PredictionService : IPredictionService
{
    private readonly IPredictionRepository _predictionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IMatchInfoClient _matchInfoClient;

    public PredictionService(
        IPredictionRepository predictionRepository,
        IWalletRepository walletRepository,
        IMatchInfoClient matchInfoClient)
    {
        _predictionRepository = predictionRepository;
        _walletRepository = walletRepository;
        _matchInfoClient = matchInfoClient;
    }

    public async Task<PredictionResponse> CreatePredictionAsync(CreatePredictionRequest request)
    {
        // 1. Validar formato del pronóstico (1 / X / 2)
        var predictionType = ParsePredictionType(request.Prediction);

        // 2. Una sola predicción por usuario y partido
        if (await _predictionRepository.ExistsForUserAndMatchAsync(request.UserId, request.MatchId))
            throw new DuplicatePredictionException();

        // 3. Consultar al Servicio de Estadísticas: el partido debe existir y no haber iniciado
        var match = await _matchInfoClient.GetMatchAsync(request.MatchId)
            ?? throw new MatchNotFoundException(request.MatchId);

        if (match.Estado != "PROGRAMADO" || match.FechaHoraUtc <= DateTime.UtcNow)
            throw new MatchAlreadyStartedException();

        // 4. Validar saldo suficiente
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId)
            ?? throw new WalletNotFoundException(request.UserId);

        if (wallet.Balance < request.Amount)
            throw new InsufficientBalanceException();

        // 5. Determinar la cuota aplicable según el pronóstico
        var odds = predictionType switch
        {
            PredictionType.Local => match.CuotaLocal,
            PredictionType.Draw => match.CuotaEmpate,
            PredictionType.Visitor => match.CuotaVisitante,
            _ => throw new InvalidPredictionValueException(request.Prediction)
        };

        // 6. Descontar saldo, registrar transacción y guardar la predicción
        wallet.Balance -= request.Amount;
        wallet.Transactions.Add(new Transaction
        {
            Type = TransactionType.Prediction,
            Amount = -request.Amount,
            Description = $"Predicción partido #{request.MatchId} ({request.Prediction})"
        });

        var prediction = new Prediction
        {
            UserId = request.UserId,
            MatchId = request.MatchId,
            PredictionType = predictionType,
            Amount = request.Amount,
            Odds = odds,
            Status = PredictionStatus.Pending
        };

        await _predictionRepository.AddAsync(prediction);
        await _predictionRepository.SaveChangesAsync(); // un solo SaveChanges: wallet y predicción comparten DbContext

        return ToResponse(prediction);
    }

    public async Task<List<PredictionResponse>> GetByUserIdAsync(int userId)
    {
        var predictions = await _predictionRepository.GetByUserIdAsync(userId);
        return predictions.Select(ToResponse).ToList();
    }

    private static PredictionType ParsePredictionType(string value) => value switch
    {
        "1" => PredictionType.Local,
        "X" or "x" => PredictionType.Draw,
        "2" => PredictionType.Visitor,
        _ => throw new InvalidPredictionValueException(value)
    };

    private static PredictionResponse ToResponse(Prediction p) => new(
        p.Id, p.UserId, p.MatchId,
        p.PredictionType.ToString(), p.Amount, p.Odds,
        p.Status.ToString(), p.CreatedAt);
}
