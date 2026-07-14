namespace UTNGolCoinApi.DTOs;
public record CreateWalletRequest(int UserId);
public record WalletResponse(int UserId, decimal Balance);
public record TransactionResponse(string Type, decimal Amount, string? Description, DateTime Date);
public record RankingEntry(int UserId, decimal Balance);
