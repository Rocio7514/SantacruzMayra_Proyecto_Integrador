using Microsoft.EntityFrameworkCore;
using UTNGolCoinApi.Data;
using UTNGolCoinApi.Middleware;
using UTNGolCoinApi.Repositories;
using UTNGolCoinApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- Controllers ----------
builder.Services.AddControllers();

// ---------- Swagger / OpenAPI ----------
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

// ---------- Base de datos (MySQL vía Pomelo) ----------
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ---------- Cliente HTTP hacia el Servicio de Estadísticas ----------
builder.Services.AddHttpClient<IMatchInfoClient, MatchInfoClient>(client =>
{
    var baseUrl = builder.Configuration["ServicioEstadisticas:BaseUrl"]
        ?? "http://localhost:8080/api/";
    client.BaseAddress = new Uri(baseUrl);
});

// ---------- Repositorios ----------
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IPredictionRepository, PredictionRepository>();
builder.Services.AddScoped<IDailyBonusRepository, DailyBonusRepository>();

// ---------- Servicios de negocio ----------
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IPredictionService, PredictionService>();
builder.Services.AddScoped<IRewardService, RewardService>();
builder.Services.AddScoped<IDailyBonusService, DailyBonusService>();

// ---------- CORS: permite que los dos frontends (Admin y Público) consuman esta API ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontends", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ---------- Middleware de manejo de errores (debe ir primero) ----------
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // disponible en /swagger
}

app.UseCors("AllowFrontends");
app.UseAuthorization();
app.MapControllers();

app.Run();
