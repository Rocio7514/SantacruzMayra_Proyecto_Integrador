using Microsoft.AspNetCore.Mvc;
using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Controllers;

[ApiController]
[Route("api/billeteras")]
public class BilleterasController : ControllerBase
{
    private readonly IBilleteraService _billeteraService;

    public BilleterasController(IBilleteraService billeteraService)
    {
        _billeteraService = billeteraService;
    }

    [HttpPost]
    public async Task<ActionResult<BilleteraResponse>> Crear([FromBody] CrearBilleteraRequest request)
    {
        var billetera = await _billeteraService.CrearBilleteraAsync(request.UsuarioId);
        return Ok(billetera);
    }

   
    [HttpGet("{usuarioId:int}")]
    public async Task<ActionResult<BilleteraResponse>> ObtenerSaldo(int usuarioId)
    {
        var billetera = await _billeteraService.ObtenerSaldoAsync(usuarioId);
        return billetera is null ? NotFound(new { mensaje = $"No existe billetera para el usuario {usuarioId}." }) : Ok(billetera);
    }
    [HttpGet("{usuarioId:int}/transacciones")]
    public async Task<ActionResult<List<TransaccionResponse>>> ObtenerTransacciones(int usuarioId)
    {
        var transacciones = await _billeteraService.ObtenerTransaccionesAsync(usuarioId);
        return transacciones is null ? NotFound(new { mensaje = $"No existe billetera para el usuario {usuarioId}." }) : Ok(transacciones);
    }
}
