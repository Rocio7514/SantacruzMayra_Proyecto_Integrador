using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Models;
using UTNGolCoinApi.Repositories;

namespace UTNGolCoinApi.Services;

public interface ILiquidacionService
{
    Task<LiquidarPremiosResponse> LiquidarAsync(int partidoId, LiquidarPremiosRequest request);
}

public class LiquidacionService : ILiquidacionService
{
    private readonly IPrediccionRepository _prediccionRepository;
    private readonly IBilleteraRepository _billeteraRepository;

    public LiquidacionService(IPrediccionRepository prediccionRepository, IBilleteraRepository billeteraRepository)
    {
        _prediccionRepository = prediccionRepository;
        _billeteraRepository = billeteraRepository;
    }

    public async Task<LiquidarPremiosResponse> LiquidarAsync(int partidoId, LiquidarPremiosRequest request)
    {
        var ganador = DeterminarGanador(request.GolesLocal, request.GolesVisitante);
        var pendientes = await _prediccionRepository.ObtenerPendientesPorPartidoIdAsync(partidoId);

        int ganadores = 0;
        decimal totalPagado = 0m;

        foreach (var prediccion in pendientes)
        {
            if (prediccion.ResultadoPronosticado == ganador)
            {
                var premio = prediccion.Monto * prediccion.CuotaAplicada;
                prediccion.Estado = EstadoPrediccion.GANADA;

                var billetera = await _billeteraRepository.ObtenerPorUsuarioIdAsync(prediccion.UsuarioId);
                if (billetera is null) continue; 

                billetera.Saldo += premio;
                billetera.Transacciones.Add(new Transaccion
                {
                    Tipo = TipoTransaccion.PREMIO_CREDITO,
                    Monto = premio,
                    Descripcion = $"Premio partido #{partidoId}"
                });

                ganadores++;
                totalPagado += premio;
            }
            else
            {
                prediccion.Estado = EstadoPrediccion.PERDIDA;
            }
        }

        await _prediccionRepository.GuardarCambiosAsync();

        return new LiquidarPremiosResponse(partidoId, pendientes.Count, ganadores, totalPagado);
    }

    private static ResultadoPronostico DeterminarGanador(int golesLocal, int golesVisitante)
    {
        if (golesLocal > golesVisitante) return ResultadoPronostico.LOCAL;
        if (golesLocal < golesVisitante) return ResultadoPronostico.VISITANTE;
        return ResultadoPronostico.EMPATE;
    }
}
