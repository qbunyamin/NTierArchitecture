using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NTierArchitecture.Entities.Abstractions;
using NTierArchitecture.Entities.Models;
using NTierArchitecture.Entities.Repositories;

namespace NTierArchitecture.Business.Features.Auth.Login;

internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LogincommandResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtProvider _jwtProvider;
    private readonly IMemoryCache _cache;
    private const string _cacheKey = "login_cache";

    public LoginCommandHandler(UserManager<AppUser> userManager, IJwtProvider jwtProvider, IMemoryCache cache)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _cache = cache;
    }

    public async Task<LogincommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {

        AppUser appUser = await _userManager.Users.Where(p => p.UserName == request.UserNameOrEmail || p.Email == request.UserNameOrEmail).FirstOrDefaultAsync(cancellationToken);

        if (appUser is null)
        {
            throw new ArgumentException("Kullanıcı bulunamadı.");

        }

        bool checkPassword = await _userManager.CheckPasswordAsync(appUser, request.Password);
        if (!checkPassword)
        {
            throw new ArgumentException("Şifre yanlış.");
        }

        string token = await _jwtProvider.CreateTokenAsync(appUser);


        if (_cache.TryGetValue(_cacheKey, out List<AppUser> cachedLogin))
        {
            // Mevcut kategori güncelleniyor
            var categoryToUpdate = _userManager.Users.FirstOrDefault(c => c.Name == request.UserNameOrEmail);
            if (categoryToUpdate != null)
            {
                categoryToUpdate.Name = request.UserNameOrEmail;

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
            var categoriesFromDb = await _userManager.Users.Where(v=>v.UserName==request.UserNameOrEmail).OrderBy(p => p.Name)// where içersinde son ekleni getir
            .ToListAsync(cancellationToken); ;
            _cache.Set(_cacheKey, categoriesFromDb, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }

        return new(AccessToken: token, UserId: appUser.Id);
    }
}