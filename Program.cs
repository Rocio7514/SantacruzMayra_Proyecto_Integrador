using Microsoft.EntityFrameworkCore;
using UTNGolCoinApi.Data;
using UTNGolCoinApi.Middleware;
using UTNGolCoinApi.Repositories;
using UTNGolCoinApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "UTNGolCoin API",
        Version = "v1",
        Description = "Servicio UTNGolCoin — billeteras, transacciones, predicciones y liquidación de premios."
    });
});

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString =
        "server=localhost;port=3306;database=utngolcoin_db;user=root;password=rocio123";
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 0))));

var servicioEstadisticasBaseUrl = builder.Configuration["ServicioEstadisticas:BaseUrl"];
if (string.IsNullOrWhiteSpace(servicioEstadisticasBaseUrl))
{
    servicioEstadisticasBaseUrl = "http://172.20.132.124:18080/demo/api/v1/";
}
var servicioEstadisticasUri = new Uri(
    $"{servicioEstadisticasBaseUrl.TrimEnd('/')}/",
    UriKind.Absolute);

builder.Services.AddHttpClient<IInfoPartidoClient, InfoPartidoClient>(client =>
{
    client.BaseAddress = servicioEstadisticasUri;
});

// Mismo servicio de Andrea, distinto endpoint (/auditoria en vez de /partidos).
builder.Services.AddHttpClient<IAuditoriaClient, AuditoriaClient>(client =>
{
    client.BaseAddress = servicioEstadisticasUri;
});

// Mismo servicio de Andrea, endpoint /usuarios/{id} (para el nombre en el ranking).
builder.Services.AddHttpClient<IUsuarioInfoClient, UsuarioInfoClient>(client =>
{
    client.BaseAddress = servicioEstadisticasUri;
});

builder.Services.AddScoped<IBilleteraRepository, BilleteraRepository>();
builder.Services.AddScoped<IPrediccionRepository, PrediccionRepository>();
builder.Services.AddScoped<IBonoDiarioRepository, BonoDiarioRepository>();
builder.Services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
builder.Services.AddScoped<IEstadoSimulacionRepository, EstadoSimulacionRepository>();

builder.Services.AddScoped<IBilleteraService, BilleteraService>();
builder.Services.AddScoped<IPrediccionService, PrediccionService>();
builder.Services.AddScoped<ILiquidacionService, LiquidacionService>();
builder.Services.AddScoped<IBonoDiarioService, BonoDiarioService>();
builder.Services.AddScoped<IConfiguracionService, ConfiguracionService>();
builder.Services.AddScoped<IReportesService, ReportesService>();
builder.Services.AddScoped<ISimulacionService, SimulacionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontends", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

await AplicarMigracionesAsync(app.Services, app.Logger);

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontends");
app.UseAuthorization();
app.MapControllers();

app.Run();

static async Task AplicarMigracionesAsync(IServiceProvider services, ILogger logger)
{
    const int maximoIntentos = 10;
    var espera = TimeSpan.FromSeconds(3);

    for (var intento = 1; intento <= maximoIntentos; intento++)
    {
        try
        {
            await using var scope = services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();
            logger.LogInformation("Migraciones de base de datos aplicadas correctamente.");
            return;
        }
        catch (Exception ex) when (intento < maximoIntentos)
        {
            logger.LogWarning(
                ex,
                "MySQL aún no está disponible. Reintento {Intento}/{MaximoIntentos} en {Segundos} segundos.",
                intento,
                maximoIntentos,
                espera.TotalSeconds);
            await Task.Delay(espera);
        }
    }
}
