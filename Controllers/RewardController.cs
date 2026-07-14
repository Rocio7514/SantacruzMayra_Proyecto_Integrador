using Microsoft.AspNetCore.Mvc;
using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Controllers;

[ApiController]
[Route("api/rewards")]
public class RewardController : ControllerBase
{
    private readonly IRewardService _rewardService;

    public RewardController(IRewardService rewardService)
    {
        _rewardService = rewardService;
    }

    /// <summary>
    /// Endpoint MÁS IMPORTANTE del servicio. Lo invoca el Servicio de
    /// Estadísticas (Persona A) cuando el administrador registra el
    /// resultado oficial de un partido. Liquida automáticamente todas
    /// las predicciones pendientes de ese partido.
    /// </summary>
    [HttpPost("process")]
    public async Task<ActionResult<ProcessRewardsResponse>> Process([FromBody] ProcessRewardsRequest request)
    {
        var result = await _rewardService.ProcessRewardsAsync(request);
        return Ok(result);
    }
}
