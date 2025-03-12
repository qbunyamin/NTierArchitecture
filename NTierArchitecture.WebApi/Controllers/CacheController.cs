using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NTierArchitecture.WebApi.Abstractions;

namespace NTierArchitecture.WebApi.Controllers;

public sealed class CacheController:ApiController
{
    private readonly IMemoryCache _cache;
    public CacheController(IMediator mediator, IMemoryCache cache) : base(mediator)
    {
        _cache = cache;
    }
 
    // örnek olarak eklendi veri cache ekleniyor...
    [HttpGet("get-cache")]
    public IActionResult GetCache()
    {
        if (_cache.TryGetValue("category_get_cache", out List<Entities.Models.Category> category))
        {
            return Ok(new { Cached = true, Data = category });
        }

        return NotFound(new { Cached = false, Message = "Cache boş." });
    }
}