using System.Text.Json;
using System.Text.Json.Serialization;

namespace UTNGolCoinApi.Services;

/// <summary>
/// Datos mínimos que necesitamos de un usuario del Servicio de
/// Estadísticas (Persona A). Solo nos interesa el nombre, para mostrarlo
/// en el ranking en vez de "Usuario X".
/// </summary>
public class UsuarioInfoDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nombre")]
    public string? Nombre { get; set; }

    [JsonPropertyName("email")]
    public string? Correo { get; set; }
}

public interface IUsuarioInfoClient
{
    Task<UsuarioInfoDto?> ObtenerUsuarioAsync(int usuarioId);
}

/// <summary>
/// Mismo servicio de Andre (misma URL base que InfoPartidoClient), pero
/// consultando /usuarios/{id} en vez de /partidos/{id}.
/// </summary>
public class UsuarioInfoClient : IUsuarioInfoClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsuarioInfoClient> _logger;

    public UsuarioInfoClient(HttpClient httpClient, ILogger<UsuarioInfoClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UsuarioInfoDto?> ObtenerUsuarioAsync(int usuarioId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"usuarios/{usuarioId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UsuarioInfoDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            // Si el servicio de Andre no responde, el ranking no debe
            // romperse: simplemente ese usuario se muestra sin nombre.
            _logger.LogWarning(ex, "No se pudo obtener el nombre del usuario {UsuarioId}", usuarioId);
            return null;
        }
    }
}
