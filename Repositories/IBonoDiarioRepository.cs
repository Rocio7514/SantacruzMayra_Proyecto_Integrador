using UTNGolCoinApi.Models;

namespace UTNGolCoinApi.Repositories;

public interface IBonoDiarioRepository
{
    Task<bool> YaOtorgadoHoyAsync(int usuarioId, DateOnly hoy);
    Task AgregarAsync(BonoDiario bono);
    Task GuardarCambiosAsync();
}
