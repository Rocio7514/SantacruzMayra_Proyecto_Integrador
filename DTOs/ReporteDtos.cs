namespace UTNGolCoinApi.DTOs;

/// <summary>Respuesta de GET /api/reportes/resumen.</summary>
public record ResumenReporteResponse(int TotalApuestas, string? PartidoMasApostado);
