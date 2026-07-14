using Microsoft.AspNetCore.Mvc;
using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Controllers;

[ApiController]
[Route("wallet")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }
    [HttpPost("create")]
    public async Task<ActionResult<WalletResponse>> Create([FromBody] CreateWalletRequest request)
    {
        var wallet = await _walletService.CreateWalletAsync(request.UserId);
        return Ok(wallet);
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<WalletResponse>> GetBalance(int userId)
    {
        var wallet = await _walletService.GetBalanceAsync(userId);
        return wallet is null ? NotFound(new { message = $"No existe billetera para el usuario {userId}." }) : Ok(wallet);
    }

    /// <summary>Historial completo de transacciones (ledger) del usuario.</summary>
    [HttpGet("{userId:int}/transactions")]
    public async Task<ActionResult<List<TransactionResponse>>> GetTransactions(int userId)
    {
        var transactions = await _walletService.GetTransactionsAsync(userId);
        return transactions is null ? NotFound(new { message = $"No existe billetera para el usuario {userId}." }) : Ok(transactions);
    }
}
