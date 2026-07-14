using Microsoft.AspNetCore.Mvc;
using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Controllers;

[ApiController]
[Route("ranking")]
public class RankingController : ControllerBase
{
    private readonly IWalletService _walletService;

    public RankingController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    public async Task<ActionResult<List<RankingEntry>>> Get()
    {
        var ranking = await _walletService.GetRankingAsync();
        return Ok(ranking);
    }
}
