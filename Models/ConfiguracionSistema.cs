namespace UTNGolCoinApi.Models;


public class ConfiguracionSistema
{
    public int Id { get; set; }
    public decimal BonoInicial { get; set; } = 10m;
    public decimal MonedasPorAcierto { get; set; } = 200m;
    public decimal LimiteMaximoApuesta { get; set; } = 500m;
    public bool ApuestasHabilitadas { get; set; } = true;
}
