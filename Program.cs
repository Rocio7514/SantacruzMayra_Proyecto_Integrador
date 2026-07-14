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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddHttpClient<IMatchInfoClient, MatchInfoClient>(client =>
{
    var baseUrl = builder.Configuration["ServicioEstadisticas:BaseUrl"]
        ?? "http://localhost:8080/api/";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IPredictionRepository, PredictionRepository>();
builder.Services.AddScoped<IDailyBonusRepository, DailyBonusRepository>();

builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IPredictionService, PredictionService>();
builder.Services.AddScoped<IRewardService, RewardService>();
builder.Services.AddScoped<IDailyBonusService, DailyBonusService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontends", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();
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
