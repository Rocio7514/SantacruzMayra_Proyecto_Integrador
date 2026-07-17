using Microsoft.AspNetCore.Mvc;
using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Controllers;

[ApiController]
[Route("api/liquidaciones")]
public class LiquidacionesController : ControllerBase
{
    private readonly ILiquidacionService _liquidacionService;

    public LiquidacionesController(ILiquidacionService liquidacionService)
    {
        _liquidacionService = liquidacionService;
    }

    [HttpPost("{partidoId:int}")]
    public async Task<ActionResult<LiquidarPremiosResponse>> Liquidar(int partidoId, [FromBody] LiquidarPremiosRequest request)
    {
        var resultado = await _liquidacionService.LiquidarAsync(partidoId, request);
        return Ok(resultado);
    }
}
