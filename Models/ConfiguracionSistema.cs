namespace UTNGolCoinApi.Models;

/// <summary>
/// Configuración global del sistema UTNGolCoin. Es una tabla de una sola
/// fila (singleton, Id fijo = 1) que el administrador puede editar desde
/// el panel administrativo.
/// </summary>
public class ConfiguracionSistema
{
    public int Id { get; set; }

    /// <summary>Monto de UTNGolCoin que recibe un usuario nuevo al registrarse.</summary>
    public decimal BonoInicial { get; set; } = 10m;

    /// <summary>Multiplicador/monedas otorgadas por cada acierto (uso configurable por el frontend).</summary>
    public decimal MonedasPorAcierto { get; set; } = 200m;

    /// <summary>Monto máximo que un usuario puede apostar en una sola predicción.</summary>
    public decimal LimiteMaximoApuesta { get; set; } = 500m;

    /// <summary>Interruptor general: si es false, el sistema no acepta nuevas predicciones.</summary>
    public bool ApuestasHabilitadas { get; set; } = true;
}
