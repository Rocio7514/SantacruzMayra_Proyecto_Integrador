using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Models;
using UTNGolCoinApi.Repositories;

namespace UTNGolCoinApi.Services;

public interface IBonoDiarioService
{
    Task<BonoDiarioResponse> OtorgarSiEsElegibleAsync(int usuarioId);
}

public class BonoDiarioService : IBonoDiarioService
{
    private const decimal MontoBonoDiario = 1m;

    private readonly IBilleteraRepository _billeteraRepository;
    private readonly IBonoDiarioRepository _bonoDiarioRepository;

    public BonoDiarioService(IBilleteraRepository billeteraRepository, IBonoDiarioRepository bonoDiarioRepository)
    {
        _billeteraRepository = billeteraRepository;
        _bonoDiarioRepository = bonoDiarioRepository;
    }

    public async Task<BonoDiarioResponse> OtorgarSiEsElegibleAsync(int usuarioId)
    {
        var billetera = await _billeteraRepository.ObtenerPorUsuarioIdAsync(usuarioId)
            ?? throw new BilleteraNoEncontradaException(usuarioId);

        if (billetera.Saldo != 0)
            throw new BonoDiarioNoElegibleException("El bono diario solo aplica cuando el saldo es 0.");

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);
        if (await _bonoDiarioRepository.YaOtorgadoHoyAsync(usuarioId, hoy))
            throw new BonoDiarioNoElegibleException("El usuario ya recibió su bono diario hoy.");

        billetera.Saldo += MontoBonoDiario;
        billetera.Transacciones.Add(new Transaccion
        {
            Tipo = TipoTransaccion.BONO_DIARIO,
            Monto = MontoBonoDiario,
            Descripcion = "Bono diario (saldo en 0)"
        });

        await _bonoDiarioRepository.AgregarAsync(new BonoDiario { UsuarioId = usuarioId, Fecha = hoy });
        await _billeteraRepository.GuardarCambiosAsync();

        return new BonoDiarioResponse(true, "Bono diario otorgado.", billetera.Saldo);
    }
}
