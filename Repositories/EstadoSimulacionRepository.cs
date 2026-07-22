using Microsoft.EntityFrameworkCore;
using UTNGolCoinApi.Data;
using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public class EstadoSimulacionRepository : IEstadoSimulacionRepository
{
    private readonly ApplicationDbContext _db;

    public EstadoSimulacionRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<EstadoSimulacion?> ObtenerAsync() =>
        _db.EstadoSimulacion.FirstOrDefaultAsync();

    public async Task AgregarAsync(EstadoSimulacion estado) =>
        await _db.EstadoSimulacion.AddAsync(estado);

    public Task GuardarCambiosAsync() => _db.SaveChangesAsync();
}
