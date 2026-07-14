using System.Net;
using UTNGolCoinApi.Services;

namespace UTNGolCoinApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var statusCode = ex switch
            {
                WalletNotFoundException or MatchNotFoundException => HttpStatusCode.NotFound,
                DuplicatePredictionException => HttpStatusCode.Conflict,
                InsufficientBalanceException
                    or MatchAlreadyStartedException
                    or InvalidPredictionValueException
                    or DailyBonusNotEligibleException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(ex, "Error no controlado procesando {Path}", context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
    }
}
