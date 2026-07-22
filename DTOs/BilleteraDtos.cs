namespace UTNGolCoinApi.DTOs;
public record CrearBilleteraRequest(int UsuarioId);
public record BilleteraResponse(int UsuarioId, decimal Saldo);
public record TransaccionResponse(string Tipo, decimal Monto, string? Descripcion, DateTime Fecha);

public record RankingEntrada(int UsuarioId, string? Nombre, string? Correo, decimal Saldo);
