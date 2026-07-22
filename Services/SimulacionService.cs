using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Models;
using UTNGolCoinApi.Repositories;

namespace UTNGolCoinApi.Services;

public interface ISimulacionService
{
    Task<AvanzarDiaResponse> AvanzarDiaAsync();
}

/// <summary>
/// Simula el paso de un día completo para poder probar la regla
/// "anti-bancarrota" sin esperar 24 horas reales. Usa su propia fecha
/// simulada (independiente del reloj del servidor), para poder avanzar
/// varios "días" seguidos en la misma sesión de pruebas.
/// </summary>
public class SimulacionService : ISimulacionService
{
    private const decimal MontoBonoDiario = 1m;

    private readonly IEstadoSimulacionRepository _estadoRepository;
    private readonly IBilleteraRepository _billeteraRepository;
    private readonly IBonoDiarioRepository _bonoDiarioRepository;

    public SimulacionService(
        IEstadoSimulacionRepository estadoRepository,
        IBilleteraRepository billeteraRepository,
        IBonoDiarioRepository bonoDiarioRepository)
    {
        _estadoRepository = estadoRepository;
        _billeteraRepository = billeteraRepository;
        _bonoDiarioRepository = bonoDiarioRepository;
    }

    public async Task<AvanzarDiaResponse> AvanzarDiaAsync()
    {
        var estado = await ObtenerOCrearEstadoAsync();

        // Avanza la fecha simulada en 1 (no la fecha real del servidor).
        estado.FechaActual = estado.FechaActual.AddDays(1);

        // Solo billeteras cuyo saldo es EXACTAMENTE 0 reciben el bono.
        // Un usuario con saldo 1 (recibido ayer y sin usar) no recibe nada
        // nuevo — así la moneda nunca se acumula, tal como se pidió.
        var billeterasEnCero = await _billeteraRepository.ObtenerConSaldoCeroAsync();

        int usuariosBeneficiados = 0;

        foreach (var billetera in billeterasEnCero)
        {
            // Defensa extra: evita otorgar dos veces el bono de la misma
            // fecha simulada a un mismo usuario si el endpoint se llamara
            // más de una vez por error.
            var yaOtorgado = await _bonoDiarioRepository.YaOtorgadoHoyAsync(billetera.UsuarioId, estado.FechaActual);
            if (yaOtorgado) continue;

            billetera.Saldo += MontoBonoDiario;
            billetera.Transacciones.Add(new Transaccion
            {
                Tipo = TipoTransaccion.BONO_DIARIO,
                Monto = MontoBonoDiario,
                Descripcion = $"Bono diario (simulacion, dia {estado.FechaActual:yyyy-MM-dd})"
            });

            await _bonoDiarioRepository.AgregarAsync(new BonoDiario
            {
                UsuarioId = billetera.UsuarioId,
                Fecha = estado.FechaActual
            });

            usuariosBeneficiados++;
        }

        await _estadoRepository.GuardarCambiosAsync(); // un solo SaveChanges para todo (estado, billeteras, transacciones y bonos)

        return new AvanzarDiaResponse(estado.FechaActual, usuariosBeneficiados, usuariosBeneficiados);
    }

    private async Task<EstadoSimulacion> ObtenerOCrearEstadoAsync()
    {
        var estado = await _estadoRepository.ObtenerAsync();
        if (estado is not null) return estado;

        estado = new EstadoSimulacion { FechaActual = DateOnly.FromDateTime(DateTime.UtcNow) };
        await _estadoRepository.AgregarAsync(estado);
        await _estadoRepository.GuardarCambiosAsync();
        return estado;
    }
}
