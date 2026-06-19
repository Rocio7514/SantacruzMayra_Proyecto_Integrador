namespace UTNGolCoinApi.DTOs;

/// <summary>Body para POST /api/billeteras — el Servicio de Estadísticas avisa que hay un usuario nuevo.</summary>
public record CrearBilleteraRequest(int UsuarioId);

/// <summary>Respuesta de saldo — GET /api/billeteras/{usuarioId}.</summary>
public record BilleteraResponse(int UsuarioId, decimal Saldo);

/// <summary>Una fila del historial — GET /api/billeteras/{usuarioId}/transacciones.</summary>
public record TransaccionResponse(string Tipo, decimal Monto, string? Descripcion, DateTime Fecha);

/// <summary>Fila de ranking — GET /api/ranking.</summary>
public record RankingEntrada(int UsuarioId, decimal Saldo);
