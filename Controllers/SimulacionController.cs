using Microsoft.AspNetCore.Mvc;
using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Controllers;

[ApiController]
[Route("api/simulacion")]
public class SimulacionController : ControllerBase
{
    private readonly ISimulacionService _simulacionService;

    public SimulacionController(ISimulacionService simulacionService)
    {
        _simulacionService = simulacionService;
    }

    /// <summary>
    /// Avanza un "día simulado". Otorga 1 UTNGolCoin a cada billetera con
    /// saldo exactamente 0 (regla anti-bancarrota). Lo llama el botón
    /// "Simular avance de 1 día" del panel administrativo (Persona C).
    /// </summary>
    [HttpPost("avanzar-dia")]
    public async Task<ActionResult<AvanzarDiaResponse>> AvanzarDia()
    {
        var resultado = await _simulacionService.AvanzarDiaAsync();
        return Ok(resultado);
    }
}
