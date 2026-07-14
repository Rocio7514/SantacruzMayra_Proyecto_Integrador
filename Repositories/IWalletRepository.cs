using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByUserIdAsync(int userId);
    Task<Wallet?> GetByUserIdWithTransactionsAsync(int userId);
    Task AddAsync(Wallet wallet);
    Task<List<Wallet>> GetAllOrderedByBalanceAsync();
    Task SaveChangesAsync();
}
