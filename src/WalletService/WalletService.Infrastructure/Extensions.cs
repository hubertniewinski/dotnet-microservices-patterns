using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WalletService.Application.Interfaces;
using WalletService.Domain.Interfaces;
using WalletService.Infrastructure.Messaging.Consumers;
using WalletService.Infrastructure.Outbox;
using WalletService.Infrastructure.Repositories;

namespace WalletService.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
        services.AddHostedService<OutboxRelay>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderPlacedConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"], "/", h =>
                {
                    h.Username(configuration["RabbitMq:Username"]);
                    h.Password(configuration["RabbitMq:Password"]);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}