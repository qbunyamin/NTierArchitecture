using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NTierArchitecture.Entities.Models;
using NTierArchitecture.Entities.Repositories;

namespace NTierArchitecture.Business.Features.Roles.CreateRole;

internal sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Unit>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;
    private const string _cacheKey = "rol_add_cache";

    public CreateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Unit> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var checkRoleIsExists = await _roleRepository.AnyAsync(p => p.Name == request.Name,cancellationToken);
        if (checkRoleIsExists)
        {
            throw new ArgumentException("Rol daha önce oluşturulmuş.");
        }

        AppRole appRole = new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        await _roleRepository.AddAsync(appRole,cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (_cache.TryGetValue(_cacheKey, out List<AppRole> cachedLogin))
        {
            // Mevcut kategori güncelleniyor
            var categoryToUpdate = _roleRepository.GetAll().FirstOrDefault(c => c.Name == request.Name);
            if (categoryToUpdate != null)
            {
                categoryToUpdate.Name = request.Name;

            }
            // Cache'i güncelle
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            _cache.Set(_cacheKey, cachedLogin, cacheOptions);
        }
        else
        {
            // Cache boşsa veritabanından veri çekip ekle
            var categoriesFromDb = await _roleRepository.GetAll().Where(v => v.Name == request.Name).OrderBy(p => p.Name)// where içersinde son ekleni getir
            .ToListAsync(cancellationToken); ;
            _cache.Set(_cacheKey, categoriesFromDb, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }

        return Unit.Value;
    }
}