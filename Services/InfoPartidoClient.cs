using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace UTNGolCoinApi.Services;

/// <summary>
/// Información mínima de un partido consultada en el Servicio de Estadísticas (Guacales).
/// Acepta el shape plano (cuotaLocal) y el anidado de Guacales (cuotas.local + fechaHora).
/// </summary>
public class InfoPartidoDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fechaHoraUtc")]
    public DateTime? FechaHoraUtc { get; set; }

    [JsonPropertyName("fechaHora")]
    public string? FechaHoraTexto { get; set; }

    [JsonPropertyName("estado")]
    public string Estado { get; set; } = "PROGRAMADO";

    [JsonPropertyName("cuotaLocal")]
    public decimal? CuotaLocalPlana { get; set; }

    [JsonPropertyName("cuotaEmpate")]
    public decimal? CuotaEmpatePlana { get; set; }

    [JsonPropertyName("cuotaVisitante")]
    public decimal? CuotaVisitantePlana { get; set; }

    [JsonPropertyName("cuotas")]
    public CuotasAnidadasDto? Cuotas { get; set; }

    public DateTime FechaHoraResuelta
    {
        get
        {
            if (FechaHoraUtc.HasValue) return FechaHoraUtc.Value;
            if (!string.IsNullOrWhiteSpace(FechaHoraTexto) &&
                DateTime.TryParse(FechaHoraTexto, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
            {
                return parsed.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc)
                    : parsed.ToUniversalTime();
            }
            return DateTime.UtcNow;
        }
    }

    public decimal CuotaLocal => CuotaLocalPlana ?? Cuotas?.Local ?? 0m;
    public decimal CuotaEmpate => CuotaEmpatePlana ?? Cuotas?.Empate ?? 0m;
    public decimal CuotaVisitante => CuotaVisitantePlana ?? Cuotas?.Visitante ?? 0m;
}

public class CuotasAnidadasDto
{
    [JsonPropertyName("local")]
    public decimal Local { get; set; }

    [JsonPropertyName("empate")]
    public decimal Empate { get; set; }

    [JsonPropertyName("visitante")]
    public decimal Visitante { get; set; }
}

public interface IInfoPartidoClient
{
    Task<InfoPartidoDto?> ObtenerPartidoAsync(int partidoId);
}

/// <summary>Cliente HTTP hacia Guacales. Aporte de integración Persona 1 ↔ Persona 2.</summary>
public class InfoPartidoClient : IInfoPartidoClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InfoPartidoClient> _logger;

    public InfoPartidoClient(HttpClient httpClient, ILogger<InfoPartidoClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<InfoPartidoDto?> ObtenerPartidoAsync(int partidoId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"partidos/{partidoId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Guacales respondió {Status} al pedir partido {PartidoId}",
                    (int)response.StatusCode,
                    partidoId);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<InfoPartidoDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo consultar el partido {PartidoId} en Estadísticas", partidoId);
            return null;
        }
    }
}
