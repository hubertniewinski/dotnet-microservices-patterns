using MassTransit;
using MassTransit.Mediator;
using Microsoft.Extensions.Logging;
using OrderService.Application.Commands.ConfirmOrder;
using Shared.Contracts.Events;

namespace OrderService.Infrastructure.Messaging.Consumers;

public class WalletDebitedConsumer : IConsumer<WalletDebitedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<WalletDebitedConsumer> _logger;

    public WalletDebitedConsumer(IMediator mediator, ILogger<WalletDebitedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<WalletDebitedEvent> context)
    {
        _logger.LogInformation("Wallet debited for Order {OrderId} — confirming order", context.Message.OrderId);

        await _mediator.Send(new ConfirmOrderCommand(context.Message.OrderId));
    }
}