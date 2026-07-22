using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace UTNGolCoinApi.Services;

/// <summary>
/// Información mínima de un partido consultada en Guacales.
/// Parseo manual (JsonDocument): Guacales manda seleccionLocal/Visitante como OBJETOS,
/// fechaHora (no solo fechaHoraUtc) y cuotas anidadas o planas.
/// </summary>
public class InfoPartidoDto
{
    public int Id { get; set; }
    public DateTime? FechaHoraUtc { get; set; }
    public string? FechaHoraTexto { get; set; }
    public string Estado { get; set; } = "PROGRAMADO";
    public decimal? CuotaLocalPlana { get; set; }
    public decimal? CuotaEmpatePlana { get; set; }
    public decimal? CuotaVisitantePlana { get; set; }
    public CuotasAnidadasDto? Cuotas { get; set; }

    /// <summary>Nombre local (extraído de seleccionLocal.nombre o string legacy).</summary>
    public string? SeleccionLocal { get; set; }

    /// <summary>Nombre visitante (extraído de seleccionVisitante.nombre o string legacy).</summary>
    public string? SeleccionVisitante { get; set; }

    public DateTime FechaHoraResuelta
    {
        get
        {
            // Hora local del equipo: ignorar el significado UTC del sufijo Z de Guacales.
            if (!string.IsNullOrWhiteSpace(FechaHoraTexto))
            {
                var limpio = FechaHoraTexto.Trim();
                if (limpio.EndsWith("Z", StringComparison.OrdinalIgnoreCase))
                {
                    limpio = limpio[..^1];
                }

                if (DateTime.TryParse(
                        limpio,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces,
                        out var parsed))
                {
                    return DateTime.SpecifyKind(parsed, DateTimeKind.Local);
                }
            }

            if (FechaHoraUtc.HasValue)
            {
                var u = FechaHoraUtc.Value;
                return new DateTime(u.Year, u.Month, u.Day, u.Hour, u.Minute, u.Second, DateTimeKind.Local);
            }

            return DateTime.Now;
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
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync($"partidos/{partidoId}");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // No se pudo contactar a Guacales (IP/puerto, apagado, timeout).
            throw new ServicioEstadisticasNoDisponibleException(ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Guacales respondió {Status} al pedir partido {PartidoId}",
                (int)response.StatusCode,
                partidoId);
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        try
        {
            return ParsearPartido(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo parsear el partido {PartidoId} de Guacales", partidoId);
            throw new ServicioEstadisticasNoDisponibleException(ex);
        }
    }

    internal static InfoPartidoDto? ParsearPartido(string json)
    {
        using var documento = JsonDocument.Parse(json);
        var raiz = documento.RootElement;
        if (raiz.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        var dto = new InfoPartidoDto
        {
            Id = LeerEntero(raiz, "id"),
            Estado = LeerCadena(raiz, "estado") ?? "PROGRAMADO",
            FechaHoraTexto = LeerCadena(raiz, "fechaHora"),
            CuotaLocalPlana = LeerDecimalNullable(raiz, "cuotaLocal"),
            CuotaEmpatePlana = LeerDecimalNullable(raiz, "cuotaEmpate"),
            CuotaVisitantePlana = LeerDecimalNullable(raiz, "cuotaVisitante"),
            SeleccionLocal = LeerNombreSeleccion(raiz, "seleccionLocal"),
            SeleccionVisitante = LeerNombreSeleccion(raiz, "seleccionVisitante")
        };

        if (raiz.TryGetProperty("fechaHoraUtc", out var fechaUtc) && fechaUtc.ValueKind == JsonValueKind.String
            && DateTime.TryParse(fechaUtc.GetString(), null, DateTimeStyles.RoundtripKind, out var utc))
        {
            dto.FechaHoraUtc = utc;
        }

        if (raiz.TryGetProperty("cuotas", out var cuotas) && cuotas.ValueKind == JsonValueKind.Object)
        {
            dto.Cuotas = new CuotasAnidadasDto
            {
                Local = LeerDecimal(cuotas, "local"),
                Empate = LeerDecimal(cuotas, "empate"),
                Visitante = LeerDecimal(cuotas, "visitante")
            };
        }

        return dto;
    }

    /// <summary>
    /// Guacales manda objeto { id, nombre, codigoPais, ... }.
    /// Versiones viejas podían mandar solo un string con el nombre.
    /// </summary>
    private static string? LeerNombreSeleccion(JsonElement raiz, string propiedad)
    {
        if (!raiz.TryGetProperty(propiedad, out var el))
        {
            return null;
        }

        if (el.ValueKind == JsonValueKind.String)
        {
            return el.GetString();
        }

        if (el.ValueKind == JsonValueKind.Object && el.TryGetProperty("nombre", out var nombre)
            && nombre.ValueKind == JsonValueKind.String)
        {
            return nombre.GetString();
        }

        return null;
    }

    private static int LeerEntero(JsonElement el, string nombre) =>
        el.TryGetProperty(nombre, out var p) && p.TryGetInt32(out var v) ? v : 0;

    private static string? LeerCadena(JsonElement el, string nombre) =>
        el.TryGetProperty(nombre, out var p) && p.ValueKind == JsonValueKind.String ? p.GetString() : null;

    private static decimal? LeerDecimalNullable(JsonElement el, string nombre) =>
        el.TryGetProperty(nombre, out var p) && p.ValueKind == JsonValueKind.Number && p.TryGetDecimal(out var v)
            ? v
            : null;

    private static decimal LeerDecimal(JsonElement el, string nombre) =>
        el.TryGetProperty(nombre, out var p) && p.ValueKind == JsonValueKind.Number && p.TryGetDecimal(out var v)
            ? v
            : 0m;
}
