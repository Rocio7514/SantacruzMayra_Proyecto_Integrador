using UTNGolCoinApi.DTOs;
using UTNGolCoinApi.Models;
using UTNGolCoinApi.Repositories;

namespace UTNGolCoinApi.Services;

public interface IConfiguracionService
{
    Task<ConfiguracionResponse> ObtenerAsync();
    Task<ConfiguracionResponse> ActualizarAsync(ActualizarConfiguracionRequest request);
}

public class ConfiguracionService : IConfiguracionService
{
    private readonly IConfiguracionRepository _configuracionRepository;

    public ConfiguracionService(IConfiguracionRepository configuracionRepository)
    {
        _configuracionRepository = configuracionRepository;
    }

    public async Task<ConfiguracionResponse> ObtenerAsync()
    {
        var configuracion = await ObtenerOCrearPorDefectoAsync();
        return AResponse(configuracion);
    }

    public async Task<ConfiguracionResponse> ActualizarAsync(ActualizarConfiguracionRequest request)
    {
        var configuracion = await ObtenerOCrearPorDefectoAsync();

        configuracion.BonoInicial = request.BonoInicial;
        configuracion.MonedasPorAcierto = request.MonedasPorAcierto;
        configuracion.LimiteMaximoApuesta = request.LimiteMaximoApuesta;
        configuracion.ApuestasHabilitadas = request.ApuestasHabilitadas;

        await _configuracionRepository.GuardarCambiosAsync();

        return AResponse(configuracion);
    }

    // La configuración es una fila única (singleton). Si todavía no existe
    // (primera vez que se usa el sistema), se crea con los valores por defecto.
    private async Task<ConfiguracionSistema> ObtenerOCrearPorDefectoAsync()
    {
        var configuracion = await _configuracionRepository.ObtenerAsync();
        if (configuracion is not null) return configuracion;

        configuracion = new ConfiguracionSistema();
        await _configuracionRepository.AgregarAsync(configuracion);
        await _configuracionRepository.GuardarCambiosAsync();
        return configuracion;
    }

    private static ConfiguracionResponse AResponse(ConfiguracionSistema c) => new(
        c.BonoInicial, c.MonedasPorAcierto, c.LimiteMaximoApuesta, c.ApuestasHabilitadas);
}
