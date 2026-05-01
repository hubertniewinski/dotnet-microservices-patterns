using MassTransit;
using MassTransit.Mediator;
using Microsoft.Extensions.Logging;
using OrderService.Application.Commands.FailOrder;
using Shared.Contracts.Events;

namespace OrderService.Infrastructure.Messaging.Consumers;

public class WalletDebitFailedConsumer : IConsumer<WalletDebitFailedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<WalletDebitFailedConsumer> _logger;

    public WalletDebitFailedConsumer(IMediator mediator, ILogger<WalletDebitFailedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<WalletDebitFailedEvent> context)
    {
        _logger.LogInformation("Wallet debit failed for Order {OrderId} — failing order", context.Message.OrderId);

        await _mediator.Send(new FailOrderCommand(context.Message.OrderId));
    }
}