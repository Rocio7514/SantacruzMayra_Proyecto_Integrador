using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public interface IPrediccionRepository
{
    Task<bool> ExisteParaUsuarioYPartidoAsync(int usuarioId, int partidoId);
    Task AgregarAsync(Prediccion prediccion);
    Task<List<Prediccion>> ObtenerPorUsuarioIdAsync(int usuarioId);
    Task<List<Prediccion>> ObtenerPendientesPorPartidoIdAsync(int partidoId);
    Task GuardarCambiosAsync();
}
