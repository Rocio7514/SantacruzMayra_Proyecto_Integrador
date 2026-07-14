namespace UTNGolCoinApi.DTOs;

/// <summary>Body para POST /wallet/create — el otro servicio avisa que hay un usuario nuevo.</summary>
public record CreateWalletRequest(int UserId);

/// <summary>Respuesta de saldo — GET /wallet/{userId}.</summary>
public record WalletResponse(int UserId, decimal Balance);

/// <summary>Una fila del historial — GET /wallet/{userId}/transactions.</summary>
public record TransactionResponse(string Type, decimal Amount, string? Description, DateTime Date);

/// <summary>Fila de ranking — GET /ranking.</summary>
public record RankingEntry(int UserId, decimal Balance);
