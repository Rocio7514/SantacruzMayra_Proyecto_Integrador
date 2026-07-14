using Microsoft.EntityFrameworkCore;
using UTNGolCoinApi.Data;
using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public class DailyBonusRepository : IDailyBonusRepository
{
    private readonly ApplicationDbContext _db;

    public DailyBonusRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<bool> AlreadyGrantedTodayAsync(int userId, DateOnly today) =>
        _db.DailyBonuses.AnyAsync(d => d.UserId == userId && d.Date == today);

    public async Task AddAsync(DailyBonus bonus) => await _db.DailyBonuses.AddAsync(bonus);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
