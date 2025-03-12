using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NTierArchitecture.Entities.Repositories;

namespace NTierArchitecture.Business.Features.Category.Get;
internal sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<Entities.Models.Category>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMemoryCache _cache;
    private const string _cacheKey = "category_get_cache";

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository, IMemoryCache cache)
    {
        _categoryRepository = categoryRepository;
        _cache = cache;
    }

    public async Task<List<Entities.Models.Category>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        // Cache kontrolü
        if (_cache.TryGetValue(_cacheKey, out List<Entities.Models.Category> cachedCategories))
        {
            return cachedCategories;
        }

        // Cache'de yoksa veritabanından çek
        var categories = await _categoryRepository.GetAll()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        // Cache'e ekle (örnek olarak 5 dakika saklanacak)
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        // Veritabanından gelen veriyi cache'e ekle
        _cache.Set(_cacheKey, categories, cacheOptions);

        return categories;
    }
}
