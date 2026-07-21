using Microsoft.AspNetCore.Mvc;
using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Controllers;

[ApiController]
[Route("api/reportes")]
public class ReportesController : ControllerBase
{
    private readonly IReportesService _reportesService;

    public ReportesController(IReportesService reportesService)
    {
        _reportesService = reportesService;
    }

    /// <summary>Resumen de estadísticas: total de apuestas y partido más apostado.</summary>
    [HttpGet("resumen")]
    public async Task<ActionResult<ResumenReporteResponse>> Resumen()
    {
        var resumen = await _reportesService.ObtenerResumenAsync();
        return Ok(resumen);
    }
}
