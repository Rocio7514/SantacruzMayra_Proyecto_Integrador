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

    /// <summary>
    /// Crea una predicción 1X2 sobre un partido. Valida saldo, que el
    /// partido no haya iniciado (consultando al Servicio de Estadísticas)
    /// y que el usuario no tenga ya una predicción para ese partido.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PredictionResponse>> Create([FromBody] CreatePredictionRequest request)
    {
        var result = await _predictionService.CreatePredictionAsync(request);
        return Ok(result);
    }

    /// <summary>Lista todas las predicciones (histórico) de un usuario.</summary>
    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<List<PredictionResponse>>> GetByUser(int userId)
    {
        var result = await _predictionService.GetByUserIdAsync(userId);
        return Ok(result);
    }
}
