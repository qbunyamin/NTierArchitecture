using Microsoft.AspNetCore.Mvc;

namespace NTierArchitecture.WebApi.Controllers;

public class LogController : ControllerBase
{

    private readonly ILogger<LogController> _logger;
    private readonly Serilog.ILogger _logger2;

    public LogController(ILogger<LogController> logger, Serilog.ILogger logger2)
    {
        _logger = logger;
        _logger2 = logger2;
    }
    public void LoggerMetod(HttpContext context, Exception ex)
    {

        var id = context.Connection.RemoteIpAddress?.ToString();
        var deger = context.Request.Path.Value;
        var message = ex.Message;

        _logger.LogTrace("Id :{Number1} ,HataYolu :{Number2},HataMesajı :{Number3}", id, deger, message);
        _logger.LogDebug("Id :{Number1} ,HataYolu :{Number2},HataMesajı :{Number3}", id, deger, message);
        _logger.LogInformation("Id :{Number1} ,HataYolu :{Number2},HataMesajı :{Number3}", id, deger, message);
        _logger.LogWarning("Id :{Number1} ,HataYolu :{Number2},HataMesajı :{Number3}", id, deger, message);
        _logger.LogError("Id :{Number1} ,HataYolu :{Number2},HataMesajı :{Number3}", id, deger, message);
        _logger.LogCritical("Id :{Number1} ,HataYolu :{Number2},HataMesajı :{Number3}", id, deger, message);
        _logger.Log(LogLevel.None, "Id :{Number1} ,HataYolu :{Number2},HataMesajı :{Number3}", id, deger, message);

    }
}
