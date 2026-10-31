
using Momentum.Application;
using Momentum.Infrastructure;

namespace Momentum.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddAppDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationDI()
                .AddInfrastructureDI(configuration);

        return services;
    }
}