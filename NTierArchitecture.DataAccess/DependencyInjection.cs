using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NTierArchitecture.DataAccess.Context;
using NTierArchitecture.DataAccess.Repositories;
using NTierArchitecture.Entities.Models;
using NTierArchitecture.Entities.Repositories;
using Scrutor;

namespace NTierArchitecture.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services,IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("SqlServer");
        services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(connectionString));
        services.AddIdentityCore<AppUser>(cfr => cfr.Password.RequireNonAlphanumeric=false).AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<IUnitOfWork>(se => se.GetRequiredService<ApplicationDbContext>());

        //services.AddScoped<ICategoryRepository, CategoryRepository>();
        //services.AddScoped<IProductRepository, ProductRepository>();
        //services.AddScoped<IUserRoleRepository, UserRoleRepository>();

        //todo üsttekileri yazmak yerine bu alttaki ile hepsini al

        services.Scan(selector => selector.FromAssemblies(
            typeof(DependencyInjection).Assembly
                ).AddClasses(publicOnly:false).UsingRegistrationStrategy(RegistrationStrategy.Skip).AsImplementedInterfaces().WithScopedLifetime());

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        return services;
    }
}

