using Microsoft.AspNetCore.Mvc;

namespace UTNGolCoinApi.Controllers;

/// <summary>
/// Salud del servicio para que el frontend y Guacales verifiquen disponibilidad.
/// </summary>
[ApiController]
[Route("api/salud")]
public class SaludController : ControllerBase
{
    [HttpGet]
    public IActionResult Obtener()
    {
        return Ok(new
        {
            estado = "OK",
            servicio = "utngolcoin",
            marcaTiempo = DateTime.UtcNow
        });
    }
}
