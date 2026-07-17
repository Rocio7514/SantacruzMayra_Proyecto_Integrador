using System.Text.Json;
using System.Text.Json.Serialization;

namespace UTNGolCoinApi.Services;

/// <summary>
/// Información mínima de un partido que este servicio necesita consultar
/// en el Servicio de Estadísticas (Persona A) para validar una predicción:
/// si ya comenzó, y las cuotas vigentes 1X2.
/// </summary>
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
}

/// <summary>
/// Cliente HTTP hacia el Servicio de Estadísticas. Se registra como
/// HttpClient con nombre "ServicioEstadisticas" en Program.cs, apuntando
/// a la URL base real que te compartió tu compañera de Persona A.
/// </summary>
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
        var response = await _httpClient.GetAsync($"partidos/{partidoId}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<InfoPartidoDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
