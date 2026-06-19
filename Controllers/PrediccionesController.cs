using Microsoft.AspNetCore.Mvc;
using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Controllers;

[ApiController]
[Route("api/predicciones")]
public class PrediccionesController : ControllerBase
{
    private readonly IPrediccionService _prediccionService;
    private readonly IBonoDiarioService _bonoDiarioService;

    public PrediccionesController(IPrediccionService prediccionService, IBonoDiarioService bonoDiarioService)
    {
        _prediccionService = prediccionService;
        _bonoDiarioService = bonoDiarioService;
    }

    [HttpPost]
    public async Task<ActionResult<PrediccionResponse>> Crear([FromBody] CrearPrediccionRequest request)
    {
        var resultado = await _prediccionService.CrearPrediccionAsync(request);
        return Ok(resultado);
    }

   
    [HttpGet("usuario/{usuarioId:int}")]
    public async Task<ActionResult<List<PrediccionResponse>>> ObtenerPorUsuario(int usuarioId)
    {
        var resultado = await _prediccionService.ObtenerPorUsuarioIdAsync(usuarioId);
        return Ok(resultado);
    }

   
    [HttpPost("bono-diario/{usuarioId:int}")]
    public async Task<ActionResult<BonoDiarioResponse>> BonoDiario(int usuarioId)
    {
        var resultado = await _bonoDiarioService.OtorgarSiEsElegibleAsync(usuarioId);
        return Ok(resultado);
    }
}
