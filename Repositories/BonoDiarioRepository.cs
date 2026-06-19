using Microsoft.EntityFrameworkCore;
using UTNGolCoinApi.Data;
using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public class BonoDiarioRepository : IBonoDiarioRepository
{
    private readonly ApplicationDbContext _db;

    public BonoDiarioRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<bool> YaOtorgadoHoyAsync(int usuarioId, DateOnly hoy) =>
        _db.BonosDiarios.AnyAsync(d => d.UsuarioId == usuarioId && d.Fecha == hoy);

    public async Task AgregarAsync(BonoDiario bono) => await _db.BonosDiarios.AddAsync(bono);

    public Task GuardarCambiosAsync() => _db.SaveChangesAsync();
}
