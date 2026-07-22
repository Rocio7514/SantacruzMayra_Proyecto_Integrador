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
    private readonly IBilleteraService _billeteraService;

    public BonoDiarioService(
        IBilleteraRepository billeteraRepository,
        IBonoDiarioRepository bonoDiarioRepository,
        IBilleteraService billeteraService)
    {
        _billeteraRepository = billeteraRepository;
        _bonoDiarioRepository = bonoDiarioRepository;
        _billeteraService = billeteraService;
    }

    public async Task<BonoDiarioResponse> OtorgarSiEsElegibleAsync(int usuarioId)
    {
        var billeteraEncontrada = await _billeteraRepository.ObtenerPorUsuarioIdAsync(usuarioId);

        if (billeteraEncontrada is null)
        {
            // Autocuración: si nunca se creó la billetera (p. ej. el Servicio
            // de Estadísticas no notificó el registro), la creamos ahora con
            // el bono de bienvenida. Como queda con saldo 10 (no 0), el bono
            // diario no aplicará todavía — y eso es correcto: el mensaje que
            // reciba el frontend pasa de "no existe billetera" (404) a
            // "el bono diario solo aplica con saldo 0" (400), que ya no
            // rompe el inicio de sesión.
            await _billeteraService.CrearBilleteraAsync(usuarioId);
            billeteraEncontrada = await _billeteraRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        }
        var billetera = billeteraEncontrada
            ?? throw new BilleteraNoEncontradaException(usuarioId); // no debería pasar nunca

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
