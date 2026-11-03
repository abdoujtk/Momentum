using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Momentum.Application.Interfaces;
using Momentum.Infrastructure.Persistence;
using Momentum.Infrastructure.Repositories;
using Momentum.Infrastructure.Services;

namespace Momentum.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<MomentumDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("MomentumDb")));

        services.AddScoped<IHabitRepository, HabitRepository>();
        services.AddScoped<ICheckInRepository, CheckInRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IMoodRepository, MoodRepository>();


        return services;
    }
}