using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Newtonsoft.Json;

namespace WalletService.Infrastructure.Outbox;

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
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var messages = await db.OutboxMessages
            .Where(m => !m.Published)
            .Take(20)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            try
            {
                var eventType = Type.GetType(message.EventType);
                if (eventType is null)
                {
                    _logger.LogWarning("Unknown event type {EventType}", message.EventType);
                    continue;
                }

                var payload = JsonConvert.DeserializeObject(message.Payload, eventType, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                if (payload is not null)
                {
                    await publishEndpoint.Publish(payload, eventType, ct);
                }

                message.Published = true;

                _logger.LogInformation("Published outbox message {EventType} {MessageId}", message.EventType, message.Id);
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