using Microsoft.EntityFrameworkCore;
using UTNGolCoinApi.Data;
using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public class PrediccionRepository : IPrediccionRepository
{
    private readonly ApplicationDbContext _db;

    public PrediccionRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExisteParaUsuarioYPartidoAsync(int usuarioId, int partidoId) =>
        _db.Predicciones.AnyAsync(p => p.UsuarioId == usuarioId && p.PartidoId == partidoId);

    public async Task AgregarAsync(Prediccion prediccion) => await _db.Predicciones.AddAsync(prediccion);

    public Task<List<Prediccion>> ObtenerPorUsuarioIdAsync(int usuarioId) =>
        _db.Predicciones.Where(p => p.UsuarioId == usuarioId)
                          .OrderByDescending(p => p.FechaCreacion)
                          .ToListAsync();

    public Task<List<Prediccion>> ObtenerPendientesPorPartidoIdAsync(int partidoId) =>
        _db.Predicciones.Where(p => p.PartidoId == partidoId && p.Estado == EstadoPrediccion.PENDIENTE)
                          .ToListAsync();

    public Task<List<Prediccion>> ObtenerTodasAsync() =>
        _db.Predicciones.ToListAsync();

    public Task GuardarCambiosAsync() => _db.SaveChangesAsync();
}
