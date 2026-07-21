namespace UTNGolCoinApi.Services;


public abstract class ExcepcionNegocio : Exception
{
    protected ExcepcionNegocio(string message) : base(message) { }
}

public class BilleteraNoEncontradaException : ExcepcionNegocio
{
    public BilleteraNoEncontradaException(int usuarioId) : base($"No existe billetera para el usuario {usuarioId}.") { }
}

public class SaldoInsuficienteException : ExcepcionNegocio
{
    public SaldoInsuficienteException() : base("Saldo insuficiente para realizar esta predicción.") { }
}

public class PrediccionDuplicadaException : ExcepcionNegocio
{
    public PrediccionDuplicadaException() : base("El usuario ya registró una predicción para este partido.") { }
}

public class PartidoNoEncontradoException : ExcepcionNegocio
{
    public PartidoNoEncontradoException(int partidoId) : base($"No se encontró el partido {partidoId} en el Servicio de Estadísticas.") { }
}

public class PartidoYaIniciadoException : ExcepcionNegocio
{
    public PartidoYaIniciadoException() : base("No se pueden registrar predicciones: el partido ya inició o finalizó.") { }
}

public class ValorPrediccionInvalidoException : ExcepcionNegocio
{
    public ValorPrediccionInvalidoException(string valor) : base($"Valor de predicción inválido: '{valor}'. Use 'LOCAL', 'EMPATE' o 'VISITANTE'.") { }
}

public class BonoDiarioNoElegibleException : ExcepcionNegocio
{
    public BonoDiarioNoElegibleException(string razon) : base(razon) { }
}
