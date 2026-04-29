using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Infrastructure.Outbox;

public class OutboxRelay : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<OutboxRelay> _logger;

    public OutboxRelay(IServiceProvider sp, ILogger<OutboxRelay> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await ProcessOutboxAsync(ct);
            await Task.Delay(TimeSpan.FromSeconds(5), ct);
        }
    }

    private async Task ProcessOutboxAsync(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var messages = await db.OutboxMessages
            .Where(m => !m.Published)
            .Take(20)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            try
            {
                // TODO: publishing to Rabbit
                _logger.LogInformation(
                    "Publishing outbox message {EventType} {MessageId}",
                    message.EventType,
                    message.Id);

                message.Published = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to publish outbox message {MessageId}",
                    message.Id);
            }
        }

        await db.SaveChangesAsync(ct);
    }
}