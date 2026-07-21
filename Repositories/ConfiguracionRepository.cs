using Microsoft.EntityFrameworkCore;
using UTNGolCoinApi.Data;
using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public class ConfiguracionRepository : IConfiguracionRepository
{
    private readonly ApplicationDbContext _db;

    public ConfiguracionRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    // Siempre es la fila con Id = 1 (configuración única del sistema).
    public Task<ConfiguracionSistema?> ObtenerAsync() =>
        _db.Configuracion.FirstOrDefaultAsync();

    public async Task AgregarAsync(ConfiguracionSistema configuracion) =>
        await _db.Configuracion.AddAsync(configuracion);

    public Task GuardarCambiosAsync() => _db.SaveChangesAsync();
}
