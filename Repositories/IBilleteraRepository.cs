using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public interface IBilleteraRepository
{
    Task<Billetera?> ObtenerPorUsuarioIdAsync(int usuarioId);
    Task<Billetera?> ObtenerPorUsuarioIdConTransaccionesAsync(int usuarioId);
    Task AgregarAsync(Billetera billetera);
    Task<List<Billetera>> ObtenerTodasOrdenadasPorSaldoAsync();
    Task<List<Billetera>> ObtenerConSaldoCeroAsync();
    Task GuardarCambiosAsync();
}
