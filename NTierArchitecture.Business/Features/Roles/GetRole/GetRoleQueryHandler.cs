using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NTierArchitecture.Entities.Models;
using NTierArchitecture.Entities.Repositories;

namespace NTierArchitecture.Business.Features.Roles.GetRole;

internal sealed class GetRoleQueryHandler : IRequestHandler<GetRoleQuery, List<GetRolesQueryResponse>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMemoryCache _cache;
    private const string _cacheKey = "rol_get_cache";

    public GetRoleQueryHandler(IRoleRepository roleRepository, IMemoryCache cache)
    {
        _roleRepository = roleRepository;
        _cache = cache;
    }
    public async Task<List<GetRolesQueryResponse>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        // Cache kontrolü
        if (_cache.TryGetValue(_cacheKey, out List<GetRolesQueryResponse> cachedGetRole))
        {
            return cachedGetRole;
        }

        // Cache'de yoksa veritabanından çek
        var response =
               await _roleRepository.GetAll()
                   .Select(r => new GetRolesQueryResponse(r.Id, r.Name))
                   .ToListAsync(cancellationToken);

        // Cache'e ekle (örnek olarak 5 dakika saklanacak)
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        // Veritabanından gelen veriyi cache'e ekle
        _cache.Set(_cacheKey, response, cacheOptions);

        return response;

    }
}