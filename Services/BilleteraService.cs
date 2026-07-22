using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Models;
using UTNGolCoinApi.Repositories;

namespace UTNGolCoinApi.Services;

public interface IBilleteraService
{
    Task<BilleteraResponse> CrearBilleteraAsync(int usuarioId);
    Task<BilleteraResponse> ObtenerSaldoAsync(int usuarioId);
    Task<List<TransaccionResponse>> ObtenerTransaccionesAsync(int usuarioId);
    Task<List<RankingEntrada>> ObtenerRankingAsync();
}

public class BilleteraService : IBilleteraService
{
    private const decimal SaldoBienvenida = 10m;

    private readonly IBilleteraRepository _billeteraRepository;
    private readonly IAuditoriaClient _auditoriaClient;
    private readonly IUsuarioInfoClient _usuarioInfoClient;

    public BilleteraService(
        IBilleteraRepository billeteraRepository,
        IAuditoriaClient auditoriaClient,
        IUsuarioInfoClient usuarioInfoClient)
    {
        _billeteraRepository = billeteraRepository;
        _auditoriaClient = auditoriaClient;
        _usuarioInfoClient = usuarioInfoClient;
    }

    public async Task<BilleteraResponse> CrearBilleteraAsync(int usuarioId)
    {
        var existente = await _billeteraRepository.ObtenerPorUsuarioIdAsync(usuarioId);
        if (existente != null)
        {
            
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

        // BUG 3 (reportado por Persona D): notificar al módulo de auditoría
        // de Andre cuando se da de alta una billetera nueva (evento más
        // cercano a "registro" que ocurre dentro de este servicio).
        await _auditoriaClient.RegistrarEventoAsync(
            accion: "REGISTRO",
            entidad: "Billetera",
            detalle: $"Se creo la billetera del usuario con id {usuarioId} (bono de bienvenida: {SaldoBienvenida} UGC)");

        return new BilleteraResponse(billetera.UsuarioId, billetera.Saldo);
    }

    public async Task<BilleteraResponse> ObtenerSaldoAsync(int usuarioId)
    {
        var billetera = await _billeteraRepository.ObtenerPorUsuarioIdAsync(usuarioId);

        // Autocuración: si el usuario ya existe en el Servicio de Estadísticas
        // pero por alguna razón nunca se disparó la notificación de registro
        // (o falló), creamos su billetera aquí mismo con el bono de bienvenida,
        // en vez de responder 404. Así el frontend nunca se queda bloqueado.
        if (billetera is null)
        {
            return await CrearBilleteraAsync(usuarioId);
        }

        return new BilleteraResponse(billetera.UsuarioId, billetera.Saldo);
    }

    public async Task<List<TransaccionResponse>> ObtenerTransaccionesAsync(int usuarioId)
    {
        var billetera = await _billeteraRepository.ObtenerPorUsuarioIdConTransaccionesAsync(usuarioId);

        if (billetera is null)
        {
            // Autocuración: crea la billetera (con su transacción de bono de
            // bienvenida) y devuelve ese historial recién creado.
            await CrearBilleteraAsync(usuarioId);
            billetera = await _billeteraRepository.ObtenerPorUsuarioIdConTransaccionesAsync(usuarioId);
        }

        return billetera!.Transacciones
            .OrderByDescending(t => t.Fecha)
            .Select(t => new TransaccionResponse(t.Tipo.ToString(), t.Monto, t.Descripcion, t.Fecha))
            .ToList();
    }

    public async Task<List<RankingEntrada>> ObtenerRankingAsync()
    {
        var billeteras = await _billeteraRepository.ObtenerTodasOrdenadasPorSaldoAsync();

        var resultado = new List<RankingEntrada>();
        foreach (var b in billeteras)
        {
            // Si Andre no responde para este usuario, el ranking sigue
            // funcionando: el nombre simplemente queda en null (el
            // frontend puede mostrar "Usuario X" solo como respaldo).
            var usuario = await _usuarioInfoClient.ObtenerUsuarioAsync(b.UsuarioId);
            resultado.Add(new RankingEntrada(b.UsuarioId, usuario?.Nombre, usuario?.Correo, b.Saldo));
        }

        return resultado;
    }
}
