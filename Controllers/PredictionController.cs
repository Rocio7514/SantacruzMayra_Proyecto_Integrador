using Microsoft.AspNetCore.Mvc;
using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Controllers;

[ApiController]
[Route("prediction")]
public class PredictionController : ControllerBase
{
    private readonly IPredictionService _predictionService;

    public PredictionController(IPredictionService predictionService)
    {
        _predictionService = predictionService;
    }
    [HttpPost]
    public async Task<ActionResult<PredictionResponse>> Create([FromBody] CreatePredictionRequest request)
    {
        var result = await _predictionService.CreatePredictionAsync(request);
        return Ok(result);
    }

    
    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<List<PredictionResponse>>> GetByUser(int userId)
    {
        var result = await _predictionService.GetByUserIdAsync(userId);
        return Ok(result);
    }
}
