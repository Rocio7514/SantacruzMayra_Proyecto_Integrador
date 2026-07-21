using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public interface IConfiguracionRepository
{
    Task<ConfiguracionSistema?> ObtenerAsync();
    Task AgregarAsync(ConfiguracionSistema configuracion);
    Task GuardarCambiosAsync();
}
