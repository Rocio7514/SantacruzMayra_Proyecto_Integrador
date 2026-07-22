using System.Text.Json;
using System.Text.Json.Serialization;

namespace UTNGolCoinApi.Services;

public class InfoPartidoDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fechaHoraUtc")]
    public DateTime FechaHoraUtc { get; set; }

    [JsonPropertyName("estado")]
    public string Estado { get; set; } = "PROGRAMADO";

    [JsonPropertyName("cuotaLocal")]
    public decimal CuotaLocal { get; set; }

    [JsonPropertyName("cuotaEmpate")]
    public decimal CuotaEmpate { get; set; }

    [JsonPropertyName("cuotaVisitante")]
    public decimal CuotaVisitante { get; set; }

    // Nombres de las selecciones (para mostrar "España vs. Argentina" en reportes).
    // Si el Servicio de Estadísticas usa otros nombres de campo, ajústalos aquí.
    [JsonPropertyName("seleccionLocal")]
    public string? SeleccionLocal { get; set; }

    [JsonPropertyName("seleccionVisitante")]
    public string? SeleccionVisitante { get; set; }
}

public interface IInfoPartidoClient
{
    Task<InfoPartidoDto?> ObtenerPartidoAsync(int partidoId);
}


public class InfoPartidoClient : IInfoPartidoClient
{
    private readonly HttpClient _httpClient;

    public InfoPartidoClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
            // No se pudo ni siquiera contactar al servicio de Andre
            // (IP/puerto incorrectos, su servicio apagado, etc.) — esto es
            // distinto a que el partido no exista.
            throw new ServicioEstadisticasNoDisponibleException(ex);
        }

        // Respondió, pero con un error (ej. 404): el partido no existe.
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<InfoPartidoDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
