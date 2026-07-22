namespace UTNGolCoinApi.Models;

/// <summary>
/// Guarda en qué "día simulado" va el sistema. Es una fila única
/// (singleton, Id fijo = 1). Cada vez que el admin presiona
/// "Simular avance de 1 día", esta fecha avanza en 1, independientemente
/// de la fecha real del servidor.
/// </summary>
public class EstadoSimulacion
{
    public int Id { get; set; }

    public DateOnly FechaActual { get; set; }
}
