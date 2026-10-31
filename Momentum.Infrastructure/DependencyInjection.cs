using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Momentum.Infrastructure.Persistence;

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

     

        return services;
    }
}