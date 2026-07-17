using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Models;
using UTNGolCoinApi.Repositories;

namespace UTNGolCoinApi.Services;

public interface IBilleteraService
{
    Task<BilleteraResponse> CrearBilleteraAsync(int usuarioId);
    Task<BilleteraResponse?> ObtenerSaldoAsync(int usuarioId);
    Task<List<TransaccionResponse>?> ObtenerTransaccionesAsync(int usuarioId);
    Task<List<RankingEntrada>> ObtenerRankingAsync();
}

public class BilleteraService : IBilleteraService
{
    private const decimal SaldoBienvenida = 10m;

    private readonly IBilleteraRepository _billeteraRepository;

    public BilleteraService(IBilleteraRepository billeteraRepository)
    {
        _billeteraRepository = billeteraRepository;
    }

    public async Task<BilleteraResponse> CrearBilleteraAsync(int usuarioId)
    {
        var existente = await _billeteraRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        if (existente != null)
        {
            // Idempotencia: si el usuario ya tiene billetera, no se duplica.
            return new BilleteraResponse(existente.UsuarioId, existente.Saldo);
        }

        var billetera = new Billetera
        {
            UsuarioId = usuarioId,
            Saldo = SaldoBienvenida
        };

        billetera.Transacciones.Add(new Transaccion
        {
            Tipo = TipoTransaccion.BONO_BIENVENIDA,
            Monto = SaldoBienvenida,
            Descripcion = "Bono de bienvenida"
        });

        await _billeteraRepository.AgregarAsync(billetera);
        await _billeteraRepository.GuardarCambiosAsync();

        return new BilleteraResponse(billetera.UsuarioId, billetera.Saldo);
    }

    public async Task<BilleteraResponse?> ObtenerSaldoAsync(int usuarioId)
    {
        var billetera = await _billeteraRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        return billetera is null ? null : new BilleteraResponse(billetera.UsuarioId, billetera.Saldo);
    }

    public async Task<List<TransaccionResponse>?> ObtenerTransaccionesAsync(int usuarioId)
    {
        var billetera = await _billeteraRepository.ObtenerPorUsuarioIdConTransaccionesAsync(usuarioId);
        if (billetera is null) return null;

        return billetera.Transacciones
            .OrderByDescending(t => t.Fecha)
            .Select(t => new TransaccionResponse(t.Tipo.ToString(), t.Monto, t.Descripcion, t.Fecha))
            .ToList();
    }

    public async Task<List<RankingEntrada>> ObtenerRankingAsync()
    {
        var billeteras = await _billeteraRepository.ObtenerTodasOrdenadasPorSaldoAsync();
        return billeteras.Select(w => new RankingEntrada(w.UsuarioId, w.Saldo)).ToList();
    }
}
