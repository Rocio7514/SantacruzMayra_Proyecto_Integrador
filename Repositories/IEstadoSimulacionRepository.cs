using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public interface IEstadoSimulacionRepository
{
    Task<EstadoSimulacion?> ObtenerAsync();
    Task AgregarAsync(EstadoSimulacion estado);
    Task GuardarCambiosAsync();
}
