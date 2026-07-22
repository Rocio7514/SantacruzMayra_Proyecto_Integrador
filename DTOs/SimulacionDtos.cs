namespace UTNGolCoinApi.DTOs;

/// <summary>Respuesta de POST /api/simulacion/avanzar-dia.</summary>
public record AvanzarDiaResponse(DateOnly FechaSimulada, int UsuariosBeneficiados, int MonedasEntregadas);
