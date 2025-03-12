
using MediatR;
using NTierArchitecture.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ErrorOr;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace NTierArchitecture.Business.Features.Category.Create;
internal sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand ,ErrorOr<Unit>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string _cacheKey = "category_add_cache";

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<ErrorOr<Unit>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var isCategoryNameExist = await _categoryRepository.AnyAsync(p => p.Name == request.Name, cancellationToken);

        if (isCategoryNameExist)
        {
            return Error.Conflict("NameIsExists","Bu kategori daha önce oluşturulmuş");
        }

        Entities.Models.Category category = _mapper.Map<Entities.Models.Category>(request);
        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (_cache.TryGetValue(_cacheKey, out List<Entities.Models.Category> cachedCategories))
        {
            // Mevcut kategori güncelleniyor
            var categoryToUpdate = cachedCategories.FirstOrDefault(c => c.Name == request.Name);
            if (categoryToUpdate != null)
            {
                categoryToUpdate.Name = request.Name;

            }
            // Cache'i güncelle
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            _cache.Set(_cacheKey, cachedCategories, cacheOptions);
        }
        else
        {
            // Cache boşsa veritabanından veri çekip ekle
            var categoriesFromDb = await _categoryRepository.GetAll().Where(v => v.Name == request.Name).OrderBy(p => p.Name)
            .ToListAsync(cancellationToken); ;
            _cache.Set(_cacheKey, categoriesFromDb, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }

        return Unit.Value;
    }
}
