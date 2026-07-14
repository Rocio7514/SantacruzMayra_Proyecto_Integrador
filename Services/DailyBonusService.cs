using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Models;
using UTNGolCoinApi.Repositories;

namespace UTNGolCoinApi.Services;

public interface IDailyBonusService
{
    Task<DailyBonusResponse> GrantIfEligibleAsync(int userId);
}

public class DailyBonusService : IDailyBonusService
{
    private const decimal MontoBonoDiario = 1m;

    private readonly IWalletRepository _walletRepository;
    private readonly IDailyBonusRepository _dailyBonusRepository;

    public DailyBonusService(IWalletRepository walletRepository, IDailyBonusRepository dailyBonusRepository)
    {
        _walletRepository = walletRepository;
        _dailyBonusRepository = dailyBonusRepository;
    }

    public async Task<DailyBonusResponse> GrantIfEligibleAsync(int userId)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(userId)
            ?? throw new WalletNotFoundException(userId);

        if (wallet.Balance != 0)
            throw new DailyBonusNotEligibleException("El bono diario solo aplica cuando el saldo es 0.");

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        if (await _dailyBonusRepository.AlreadyGrantedTodayAsync(userId, hoy))
            throw new DailyBonusNotEligibleException("El usuario ya recibió su bono diario hoy.");

        wallet.Balance += MontoBonoDiario;
        wallet.Transactions.Add(new Transaction
        {
            Type = TransactionType.DailyBonus,
            Amount = MontoBonoDiario,
            Description = "Bono diario (saldo en 0)"
        });

        await _dailyBonusRepository.AddAsync(new DailyBonus { UserId = userId, Date = hoy });
        await _walletRepository.SaveChangesAsync();

        return new DailyBonusResponse(true, "Bono diario otorgado.", wallet.Balance);
    }
}
