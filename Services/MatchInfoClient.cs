using System.Text.Json;
using System.Text.Json.Serialization;

namespace UTNGolCoinApi.Services;

/// <summary>
/// Representa la información mínima de un partido que este servicio necesita
/// consultar en el Servicio de Estadísticas (Persona A) para validar una
/// predicción: si ya comenzó, y las cuotas vigentes 1X2.
/// </summary>
public class MatchInfoDto
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
/// a la IP real de la Persona A en la red del equipo.
/// </summary>
public interface IMatchInfoClient
{
    Task<MatchInfoDto?> GetMatchAsync(int matchId);
}

public class MatchInfoClient : IMatchInfoClient
{
    private readonly HttpClient _httpClient;

    public MatchInfoClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<MatchInfoDto?> GetMatchAsync(int matchId)
    {
        var response = await _httpClient.GetAsync($"partidos/{matchId}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MatchInfoDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
