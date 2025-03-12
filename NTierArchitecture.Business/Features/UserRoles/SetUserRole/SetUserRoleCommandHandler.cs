using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NTierArchitecture.Entities.Models;
using NTierArchitecture.Entities.Repositories;

namespace NTierArchitecture.Business.Features.UserRoles.SetUserRole;

internal sealed class SetUserRoleCommandHandler : IRequestHandler<SetUserRoleCommand, Unit>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;
    private const string _cacheKey = "set_user_role_cache";

    public SetUserRoleCommandHandler(IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Unit> Handle(SetUserRoleCommand request, CancellationToken cancellationToken)
    {
        var checkIsROleExists =
            await _userRoleRepository
                .AnyAsync(p => p.AppUserId == request.UserId && p.AppRoleId == request.RoleId,cancellationToken);
        if (checkIsROleExists)
        {
            throw new ArgumentException("Bu rol daha önce eklenmiş.");
        }

        UserRole userRole = new()
        {
            AppRoleId = request.RoleId,
            AppUserId = request.UserId,
        };

        await _userRoleRepository.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (_cache.TryGetValue(_cacheKey, out List<UserRole> cachedSetUsersRole))
        {
            // Mevcut kategori güncelleniyor
            var categoryToUpdate = cachedSetUsersRole.FirstOrDefault(c => c.AppRoleId == request.RoleId);
            if (categoryToUpdate != null)
            {
                categoryToUpdate.AppRoleId = request.RoleId;

            }
            // Cache'i güncelle
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            _cache.Set(_cacheKey, cachedSetUsersRole, cacheOptions);
        }
        else
        {
            // Cache boşsa veritabanından veri çekip ekle
            var categoriesFromDb = await _userRoleRepository.GetAll().Where(v => v.AppRoleId == request.RoleId).OrderBy(p => p.AppRoleId)
            .ToListAsync(cancellationToken); ;
            _cache.Set(_cacheKey, categoriesFromDb, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }

        return Unit.Value;
    }
}