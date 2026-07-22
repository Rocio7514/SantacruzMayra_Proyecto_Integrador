using Microsoft.EntityFrameworkCore;
using UTNGolCoinApi.Data;
using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public class BilleteraRepository : IBilleteraRepository
{
    private readonly ApplicationDbContext _db;

    public BilleteraRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Billetera?> ObtenerPorUsuarioIdAsync(int usuarioId) =>
        _db.Billeteras.FirstOrDefaultAsync(w => w.UsuarioId == usuarioId);

    public Task<Billetera?> ObtenerPorUsuarioIdConTransaccionesAsync(int usuarioId) =>
        _db.Billeteras.Include(w => w.Transacciones)
                       .FirstOrDefaultAsync(w => w.UsuarioId == usuarioId);

    public async Task AgregarAsync(Billetera billetera) => await _db.Billeteras.AddAsync(billetera);

    public Task<List<Billetera>> ObtenerTodasOrdenadasPorSaldoAsync() =>
        _db.Billeteras.OrderByDescending(w => w.Saldo).ToListAsync();

    // Incluye las transacciones porque la simulación necesita agregar el
    // movimiento del bono a cada billetera beneficiada.
    public Task<List<Billetera>> ObtenerConSaldoCeroAsync() =>
        _db.Billeteras.Include(w => w.Transacciones)
                       .Where(w => w.Saldo == 0)
                       .ToListAsync();

    public Task GuardarCambiosAsync() => _db.SaveChangesAsync();
}
