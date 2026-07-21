namespace UTNGolCoinApi.Models;

public class BonoDiario
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public DateOnly Fecha { get; set; }
}
