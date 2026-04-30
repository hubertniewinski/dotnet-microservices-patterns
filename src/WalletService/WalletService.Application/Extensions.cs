using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WalletService.Application.Behaviors;
using FluentValidation;

namespace WalletService.Application;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(Extensions).Assembly));

        services.AddValidatorsFromAssembly(
            typeof(Extensions).Assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));

        return services;
    }
}