namespace UTNGolCoinApi.DTOs;
public record CrearPrediccionRequest(int UsuarioId, int PartidoId, string ResultadoPronosticado, decimal Monto);
public record PrediccionResponse(
    int Id,
    int UsuarioId,
    int PartidoId,
    string ResultadoPronosticado,
    decimal Monto,
    decimal CuotaAplicada,
    string Estado,
    DateTime FechaCreacion);
