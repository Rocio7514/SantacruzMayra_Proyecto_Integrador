using Microsoft.EntityFrameworkCore;
using UTNGolCoinApi.Data;
using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly ApplicationDbContext _db;

    public WalletRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Wallet?> GetByUserIdAsync(int userId) =>
        _db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);

    public Task<Wallet?> GetByUserIdWithTransactionsAsync(int userId) =>
        _db.Wallets.Include(w => w.Transactions)
                   .FirstOrDefaultAsync(w => w.UserId == userId);

    public async Task AddAsync(Wallet wallet) => await _db.Wallets.AddAsync(wallet);

    public Task<List<Wallet>> GetAllOrderedByBalanceAsync() =>
        _db.Wallets.OrderByDescending(w => w.Balance).ToListAsync();

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
