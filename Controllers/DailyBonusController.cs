using Microsoft.AspNetCore.Mvc;
using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Controllers;

[ApiController]
[Route("dailybonus")]
public class DailyBonusController : ControllerBase
{
    private readonly IDailyBonusService _dailyBonusService;

    public DailyBonusController(IDailyBonusService dailyBonusService)
    {
        _dailyBonusService = dailyBonusService;
    }
    [HttpPost("{userId:int}")]
    public async Task<ActionResult<DailyBonusResponse>> Grant(int userId)
    {
        var result = await _dailyBonusService.GrantIfEligibleAsync(userId);
        return Ok(result);
    }
}
