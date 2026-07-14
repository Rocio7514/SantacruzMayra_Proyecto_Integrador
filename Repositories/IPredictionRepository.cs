using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public interface IPredictionRepository
{
    Task<bool> ExistsForUserAndMatchAsync(int userId, int matchId);
    Task AddAsync(Prediction prediction);
    Task<List<Prediction>> GetByUserIdAsync(int userId);
    Task<List<Prediction>> GetPendingByMatchIdAsync(int matchId);
    Task SaveChangesAsync();
}
