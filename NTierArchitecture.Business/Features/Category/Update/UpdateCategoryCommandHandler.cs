
using MediatR;
using NTierArchitecture.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace NTierArchitecture.Business.Features.Category.Update;
internal sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string _cacheKey = "category_update_cache";

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(_cacheKey, out List<Entities.Models.Category> cachedCategories))
        {
            // Mevcut kategori güncelleniyor
            var categoryToUpdate = cachedCategories.FirstOrDefault(c => c.Id == request.Id);
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
            // Cache boşsa veritabanından veri çekip ekle  // where ekle cachte sadece update olanı tut
            var categoriesFromDb = await _categoryRepository.GetAll().Where(v => v.Name == request.Name).OrderBy(p => p.Name)
            .ToListAsync(cancellationToken); ;
            _cache.Set(_cacheKey, categoriesFromDb, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }
        Entities.Models.Category category =
            await _categoryRepository.GetByIdAsync(p => p.Id == request.Id, cancellationToken);
        if (category is null)
        {
            throw new ArgumentException("Bu katogori güncellenmiş...");
        }

        if (category.Name != request.Name)
        {
            var isCategoryNameExist = await _categoryRepository.AnyAsync(p => p.Name == request.Name, cancellationToken);

            if (isCategoryNameExist)
            {
                throw new ArgumentException("Bu katogori eklenmiş...");
            }

            _mapper.Map(request, category);//değişen alanları set eder

            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }


    }
}