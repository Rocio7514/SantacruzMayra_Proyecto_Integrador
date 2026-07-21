using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Repositories;

namespace UTNGolCoinApi.Services;

public interface IReportesService
{
    Task<ResumenReporteResponse> ObtenerResumenAsync();
}

public class ReportesService : IReportesService
{
    private readonly IPrediccionRepository _prediccionRepository;
    private readonly IInfoPartidoClient _infoPartidoClient;

    public ReportesService(IPrediccionRepository prediccionRepository, IInfoPartidoClient infoPartidoClient)
    {
        _prediccionRepository = prediccionRepository;
        _infoPartidoClient = infoPartidoClient;
    }

    public async Task<ResumenReporteResponse> ObtenerResumenAsync()
    {
        var predicciones = await _prediccionRepository.ObtenerTodasAsync();
        var totalApuestas = predicciones.Count;

        if (totalApuestas == 0)
            return new ResumenReporteResponse(0, null);

        // Partido con más predicciones registradas (sin importar el estado).
        var partidoTop = predicciones
            .GroupBy(p => p.PartidoId)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;

        var nombrePartido = await ArmarNombrePartidoAsync(partidoTop);

        return new ResumenReporteResponse(totalApuestas, nombrePartido);
    }

    // Intenta obtener los nombres reales de las selecciones desde el Servicio
    // de Estadísticas. Si no responde o no trae esos campos, cae a "Partido #{id}".
    private async Task<string> ArmarNombrePartidoAsync(int partidoId)
    {
        var partido = await _infoPartidoClient.ObtenerPartidoAsync(partidoId);

        if (partido is not null
            && !string.IsNullOrWhiteSpace(partido.SeleccionLocal)
            && !string.IsNullOrWhiteSpace(partido.SeleccionVisitante))
        {
            return $"{partido.SeleccionLocal} vs. {partido.SeleccionVisitante}";
        }

        return $"Partido #{partidoId}";
    }
}
