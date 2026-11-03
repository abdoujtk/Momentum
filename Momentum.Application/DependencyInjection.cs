using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Momentum.Application.Common.Behaviors;
using Momentum.Application.Interfaces;
using Momentum.Application.Services;


namespace Momentum.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDI(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IFreezeQuotaService, FreezeQuotaService>();


        return services;
    }
}