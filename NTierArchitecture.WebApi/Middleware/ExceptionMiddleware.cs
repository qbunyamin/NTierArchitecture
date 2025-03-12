using MediatR;
using Newtonsoft.Json;
using NTierArchitecture.WebApi.Controllers;

namespace NTierArchitecture.WebApi.Middleware;

public sealed class ExceptionMiddleware :IMiddleware
{
    private readonly LogController _loggggg;

    public ExceptionMiddleware( LogController loggggg)
    {
        _loggggg = loggggg ?? throw new ArgumentNullException(nameof(loggggg));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = 500;// hata kodu verilmeli

        _loggggg.LoggerMetod(context,ex);// hatayı logla

        return context.Response.WriteAsync(new ErrorResult
        {
            Message = ex.InnerException.Message
        }.ToString());

    }
}

public class ErrorResult
{
    public string Message { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
