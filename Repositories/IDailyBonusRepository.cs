using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public interface IDailyBonusRepository
{
    Task<bool> AlreadyGrantedTodayAsync(int userId, DateOnly today);
    Task AddAsync(DailyBonus bonus);
    Task SaveChangesAsync();
}
