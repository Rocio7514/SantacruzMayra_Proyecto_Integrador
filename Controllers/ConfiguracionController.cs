using Microsoft.AspNetCore.Mvc;
using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Controllers;

[ApiController]
[Route("api/configuracion")]
public class ConfiguracionController : ControllerBase
{
    private readonly IConfiguracionService _configuracionService;

    public ConfiguracionController(IConfiguracionService configuracionService)
    {
        _configuracionService = configuracionService;
    }

    /// <summary>Devuelve la configuración actual del sistema.</summary>
    [HttpGet]
    public async Task<ActionResult<ConfiguracionResponse>> Obtener()
    {
        var configuracion = await _configuracionService.ObtenerAsync();
        return Ok(configuracion);
    }

    /// <summary>Actualiza la configuración del sistema (panel administrativo).</summary>
    [HttpPut]
    public async Task<ActionResult<ConfiguracionResponse>> Actualizar([FromBody] ActualizarConfiguracionRequest request)
    {
        var configuracion = await _configuracionService.ActualizarAsync(request);
        return Ok(configuracion);
    }
}
