using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Models;
using UTNGolCoinApi.Repositories;

namespace UTNGolCoinApi.Services;

public interface IPrediccionService
{
    Task<PrediccionResponse> CrearPrediccionAsync(CrearPrediccionRequest request);
    Task<List<PrediccionResponse>> ObtenerPorUsuarioIdAsync(int usuarioId);
}

public class PrediccionService : IPrediccionService
{
    private readonly IPrediccionRepository _prediccionRepository;
    private readonly IBilleteraRepository _billeteraRepository;
    private readonly IInfoPartidoClient _infoPartidoClient;

    public PrediccionService(
        IPrediccionRepository prediccionRepository,
        IBilleteraRepository billeteraRepository,
        IInfoPartidoClient infoPartidoClient)
    {
        _prediccionRepository = prediccionRepository;
        _billeteraRepository = billeteraRepository;
        _infoPartidoClient = infoPartidoClient;
    }

    public async Task<PrediccionResponse> CrearPrediccionAsync(CrearPrediccionRequest request)
    {
   
        var resultadoPronosticado = ParsearResultado(request.ResultadoPronosticado);

        if (await _prediccionRepository.ExisteParaUsuarioYPartidoAsync(request.UsuarioId, request.PartidoId))
            throw new PrediccionDuplicadaException();

        var partido = await _infoPartidoClient.ObtenerPartidoAsync(request.PartidoId)
            ?? throw new PartidoNoEncontradoException(request.PartidoId);

        if (partido.Estado != "PROGRAMADO" || partido.FechaHoraUtc <= DateTime.UtcNow)
            throw new PartidoYaIniciadoException();

        var billetera = await _billeteraRepository.ObtenerPorUsuarioIdAsync(request.UsuarioId)
            ?? throw new BilleteraNoEncontradaException(request.UsuarioId);

        if (billetera.Saldo < request.Monto)
            throw new SaldoInsuficienteException();

        var cuota = resultadoPronosticado switch
        {
            ResultadoPronostico.LOCAL => partido.CuotaLocal,
            ResultadoPronostico.EMPATE => partido.CuotaEmpate,
            ResultadoPronostico.VISITANTE => partido.CuotaVisitante,
            _ => throw new ValorPrediccionInvalidoException(request.ResultadoPronosticado)
        };

        billetera.Saldo -= request.Monto;
        billetera.Transacciones.Add(new Transaccion
        {
            Tipo = TipoTransaccion.PREDICCION_DEBITO,
            Monto = -request.Monto,
            Descripcion = $"Predicción partido #{request.PartidoId} ({request.ResultadoPronosticado})"
        });

        var prediccion = new Prediccion
        {
            UsuarioId = request.UsuarioId,
            PartidoId = request.PartidoId,
            ResultadoPronosticado = resultadoPronosticado,
            Monto = request.Monto,
            CuotaAplicada = cuota,
            Estado = EstadoPrediccion.PENDIENTE
        };

        await _prediccionRepository.AgregarAsync(prediccion);
        await _prediccionRepository.GuardarCambiosAsync(); 

        return AResponse(prediccion);
    }

    public async Task<List<PrediccionResponse>> ObtenerPorUsuarioIdAsync(int usuarioId)
    {
        var predicciones = await _prediccionRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        return predicciones.Select(AResponse).ToList();
    }

    private static ResultadoPronostico ParsearResultado(string valor) => valor.ToUpperInvariant() switch
    {
        "LOCAL" or "1" => ResultadoPronostico.LOCAL,
        "EMPATE" or "X" => ResultadoPronostico.EMPATE,
        "VISITANTE" or "2" => ResultadoPronostico.VISITANTE,
        _ => throw new ValorPrediccionInvalidoException(valor)
    };

    private static PrediccionResponse AResponse(Prediccion p) => new(
        p.Id, p.UsuarioId, p.PartidoId,
        p.ResultadoPronosticado.ToString(), p.Monto, p.CuotaAplicada,
        p.Estado.ToString(), p.FechaCreacion);
}
