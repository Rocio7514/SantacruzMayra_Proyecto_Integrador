namespace UTNGolCoinApi.Services;

/// <summary>Excepción base para errores de reglas de negocio (se traduce a HTTP 400/404/409).</summary>
public abstract class BusinessException : Exception
{
    protected BusinessException(string message) : base(message) { }
}

public class WalletNotFoundException : BusinessException
{
    public WalletNotFoundException(int userId) : base($"No existe billetera para el usuario {userId}.") { }
}

public class InsufficientBalanceException : BusinessException
{
    public InsufficientBalanceException() : base("Saldo insuficiente para realizar esta predicción.") { }
}

public class DuplicatePredictionException : BusinessException
{
    public DuplicatePredictionException() : base("El usuario ya registró una predicción para este partido.") { }
}

public class MatchNotFoundException : BusinessException
{
    public MatchNotFoundException(int matchId) : base($"No se encontró el partido {matchId} en el Servicio de Estadísticas.") { }
}

public class MatchAlreadyStartedException : BusinessException
{
    public MatchAlreadyStartedException() : base("No se pueden registrar predicciones: el partido ya inició o finalizó.") { }
}

public class InvalidPredictionValueException : BusinessException
{
    public InvalidPredictionValueException(string value) : base($"Valor de predicción inválido: '{value}'. Use '1', 'X' o '2'.") { }
}

public class DailyBonusNotEligibleException : BusinessException
{
    public DailyBonusNotEligibleException(string reason) : base(reason) { }
}
