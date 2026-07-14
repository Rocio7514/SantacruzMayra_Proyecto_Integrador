using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Models;
using UTNGolCoinApi.Repositories;

namespace UTNGolCoinApi.Services;

public interface IWalletService
{
    Task<WalletResponse> CreateWalletAsync(int userId);
    Task<WalletResponse?> GetBalanceAsync(int userId);
    Task<List<TransactionResponse>?> GetTransactionsAsync(int userId);
    Task<List<RankingEntry>> GetRankingAsync();
}

public class WalletService : IWalletService
{
    private const decimal SaldoBienvenida = 10m;

    private readonly IWalletRepository _walletRepository;

    public WalletService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<WalletResponse> CreateWalletAsync(int userId)
    {
        var existente = await _walletRepository.GetByUserIdAsync(userId);
        if (existente != null)
        {
            // Idempotencia: si el usuario ya tiene billetera, no se duplica.
            return new WalletResponse(existente.UserId, existente.Balance);
        }

        var wallet = new Wallet
        {
            UserId = userId,
            Balance = SaldoBienvenida
        };

        wallet.Transactions.Add(new Transaction
        {
            Type = TransactionType.WelcomeBonus,
            Amount = SaldoBienvenida,
            Description = "Bono de bienvenida"
        });

        await _walletRepository.AddAsync(wallet);
        await _walletRepository.SaveChangesAsync();

        return new WalletResponse(wallet.UserId, wallet.Balance);
    }

    public async Task<WalletResponse?> GetBalanceAsync(int userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId);
        return wallet is null ? null : new WalletResponse(wallet.UserId, wallet.Balance);
    }

    public async Task<List<TransactionResponse>?> GetTransactionsAsync(int userId)
    {
        var wallet = await _walletRepository.GetByUserIdWithTransactionsAsync(userId);
        if (wallet is null) return null;

        return wallet.Transactions
            .OrderByDescending(t => t.Date)
            .Select(t => new TransactionResponse(t.Type.ToString(), t.Amount, t.Description, t.Date))
            .ToList();
    }

    public async Task<List<RankingEntry>> GetRankingAsync()
    {
        var wallets = await _walletRepository.GetAllOrderedByBalanceAsync();
        return wallets.Select(w => new RankingEntry(w.UserId, w.Balance)).ToList();
    }
}
