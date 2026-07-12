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


builder.Services.AddHttpClient<IInfoPartidoClient, InfoPartidoClient>(client =>
{
    var baseUrl = builder.Configuration["ServicioEstadisticas:BaseUrl"]
        ?? "http://localhost:8080/demo/api/v1/";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
});


builder.Services.AddScoped<IBilleteraRepository, BilleteraRepository>();
builder.Services.AddScoped<IPrediccionRepository, PrediccionRepository>();
builder.Services.AddScoped<IBonoDiarioRepository, BonoDiarioRepository>();

builder.Services.AddScoped<IBilleteraService, BilleteraService>();
builder.Services.AddScoped<IPrediccionService, PrediccionService>();
builder.Services.AddScoped<ILiquidacionService, LiquidacionService>();
builder.Services.AddScoped<IBonoDiarioService, BonoDiarioService>();

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

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("AllowFrontends");
app.UseAuthorization();
app.MapControllers();

app.Run();
