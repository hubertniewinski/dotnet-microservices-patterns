using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Http;
using OrderService.Infrastructure.Messaging.Consumers;
using OrderService.Infrastructure.Outbox;
using OrderService.Infrastructure.Repositories;
using Polly;
using Polly.Extensions.Http;

namespace OrderService.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddHostedService<OutboxRelay>();

        var jitter = new Random();

        services.AddHttpClient<IWalletClient, WalletClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["WalletService:BaseUrl"]!);
            client.Timeout = TimeSpan.FromMilliseconds(500);
        })
        .AddPolicyHandler((serviceProvider, _) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<WalletClient>>();

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, breakDelay) =>
                    {
                        logger.LogWarning(
                            "Circuit opened for {BreakDelay}s due to {Reason}",
                            breakDelay.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                    },
                    onReset: () => logger.LogInformation("Circuit closed"),
                    onHalfOpen: () => logger.LogInformation("Circuit half-open — trial request"));
        })
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                + TimeSpan.FromMilliseconds(jitter.Next(0, 300))));

        services.AddMassTransit(x =>
        {
            x.AddConsumer<WalletDebitedConsumer>();
            x.AddConsumer<WalletDebitFailedConsumer>();

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