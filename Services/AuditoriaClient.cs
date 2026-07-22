using System.Net.Http.Json;

namespace UTNGolCoinApi.Services;

/// <summary>
/// Cliente HTTP hacia el endpoint de auditoría del Servicio de Estadísticas
/// (Persona A, Java). No se envía usuarioId a propósito: la tabla de
/// auditoría de Andre tiene una llave foránea a SU tabla de usuarios, no a
/// la nuestra — mandar un id numérico rompería esa relación. El dato
/// identificador va como texto libre dentro de "detalle".
/// </summary>
public interface IAuditoriaClient
{
    Task RegistrarEventoAsync(string accion, string entidad, string detalle);
}

public class AuditoriaClient : IAuditoriaClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuditoriaClient> _logger;

    public AuditoriaClient(HttpClient httpClient, ILogger<AuditoriaClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task RegistrarEventoAsync(string accion, string entidad, string detalle)
    {
        try
        {
            var payload = new { accion, entidad, detalle };
            var respuesta = await _httpClient.PostAsJsonAsync("auditoria", payload);

            if (!respuesta.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "El servicio de auditoria respondio {StatusCode} para la accion {Accion}",
                    respuesta.StatusCode, accion);
            }
        }
        catch (Exception ex)
        {
            // No debe romper el flujo principal (crear billetera) si el
            // servicio de auditoria no responde o cambia de IP.
            _logger.LogWarning(ex, "No se pudo notificar el evento de auditoria: {Accion}", accion);
        }
    }
}
