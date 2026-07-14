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

    [HttpPost("process")]
    public async Task<ActionResult<ProcessRewardsResponse>> Process([FromBody] ProcessRewardsRequest request)
    {
        var result = await _rewardService.ProcessRewardsAsync(request);
        return Ok(result);
    }
}
