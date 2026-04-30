using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WalletService.Domain.Interfaces;
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
        services.AddHostedService<OutboxRelay>();

        return services;
    }
}