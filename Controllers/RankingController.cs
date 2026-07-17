using Microsoft.AspNetCore.Mvc;
using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Controllers;

[ApiController]
[Route("api/ranking")]
public class RankingController : ControllerBase
{
    private readonly IBilleteraService _billeteraService;

    public RankingController(IBilleteraService billeteraService)
    {
        _billeteraService = billeteraService;
    }

   
    [HttpGet]
    public async Task<ActionResult<List<RankingEntrada>>> Obtener()
    {
        var ranking = await _billeteraService.ObtenerRankingAsync();
        return Ok(ranking);
    }
}
