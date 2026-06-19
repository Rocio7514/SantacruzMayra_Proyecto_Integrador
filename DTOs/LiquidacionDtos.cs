namespace UTNGolCoinApi.DTOs;
public record LiquidarPremiosRequest(int GolesLocal, int GolesVisitante);

public record LiquidarPremiosResponse(int PartidoId, int PrediccionesEvaluadas, int Ganadores, decimal TotalPagado);

public record BonoDiarioResponse(bool Otorgado, string Mensaje, decimal NuevoSaldo);
