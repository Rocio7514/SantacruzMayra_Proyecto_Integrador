using Microsoft.EntityFrameworkCore;
using UTNGolCoinApi.Data;
using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public class PredictionRepository : IPredictionRepository
{
    private readonly ApplicationDbContext _db;

    public PredictionRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExistsForUserAndMatchAsync(int userId, int matchId) =>
        _db.Predictions.AnyAsync(p => p.UserId == userId && p.MatchId == matchId);

    public async Task AddAsync(Prediction prediction) => await _db.Predictions.AddAsync(prediction);

    public Task<List<Prediction>> GetByUserIdAsync(int userId) =>
        _db.Predictions.Where(p => p.UserId == userId)
                        .OrderByDescending(p => p.CreatedAt)
                        .ToListAsync();

    public Task<List<Prediction>> GetPendingByMatchIdAsync(int matchId) =>
        _db.Predictions.Where(p => p.MatchId == matchId && p.Status == PredictionStatus.Pending)
                        .ToListAsync();

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
