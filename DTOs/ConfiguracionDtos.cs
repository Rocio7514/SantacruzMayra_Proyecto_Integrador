namespace UTNGolCoinApi.DTOs;

/// <summary>Respuesta de GET /api/configuracion.</summary>
public record ConfiguracionResponse(
    decimal BonoInicial,
    decimal MonedasPorAcierto,
    decimal LimiteMaximoApuesta,
    bool ApuestasHabilitadas);

/// <summary>Body para PUT /api/configuracion.</summary>
public record ActualizarConfiguracionRequest(
    decimal BonoInicial,
    decimal MonedasPorAcierto,
    decimal LimiteMaximoApuesta,
    bool ApuestasHabilitadas);
