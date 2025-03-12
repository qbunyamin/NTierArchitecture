using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NTierArchitecture.Entities.Events.Users;
using NTierArchitecture.Entities.Models;

namespace NTierArchitecture.Business.Features.Auth.Register;

internal sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMediator _mediator;
    private readonly IMemoryCache _cache;
    private const string _cacheKey = "register_cache";

    public RegisterCommandHandler(UserManager<AppUser> userManager, IMediator mediator, IMemoryCache cache)
    {
        _userManager = userManager;
        _mediator = mediator;
        _cache = cache;
    }

    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var checkUserNameExists = await _userManager.FindByNameAsync(request.UserName);
        if (checkUserNameExists is not null)
        {
            throw new ArgumentException("Kullanıcı adı mevcut.");
        }
        var checkEmailExists = await _userManager.FindByNameAsync(request.Email);
        if (checkEmailExists is not null)
        {
            throw new ArgumentException("Email adı mevcut.");
        }

        AppUser appUser = new()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.name,
            Lastname = request.lastName,
            UserName = request.UserName
        };

        await _userManager.CreateAsync(appUser, request.Password);
        await _mediator.Publish(new UserDomainEvent(appUser));


        if (_cache.TryGetValue(_cacheKey, out List<AppUser> cachedRegister))
        {
            // Mevcut kategori güncelleniyor
            var categoryToUpdate = _userManager.Users.FirstOrDefault(c => c.Name == request.UserName);
            if (categoryToUpdate != null)
            {
                categoryToUpdate.Name = request.UserName;

            }
            // Cache'i güncelle
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            _cache.Set(_cacheKey, cachedRegister, cacheOptions);
        }
        else
        {
            // Cache boşsa veritabanından veri çekip ekle
            var categoriesFromDb = await _userManager.Users.Where(v => v.UserName == request.UserName).OrderBy(p => p.Name)// where içersinde son ekleni getir
            .ToListAsync(cancellationToken); ;
            _cache.Set(_cacheKey, categoriesFromDb, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }

        return Unit.Value;

    }
}