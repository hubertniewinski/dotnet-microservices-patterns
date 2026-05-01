using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;
using WalletService.Application.Commands.DebitWallet;

namespace WalletService.Infrastructure.Messaging.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(IMediator mediator, ILogger<OrderPlacedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received OrderPlaced for Order {OrderId}", message.OrderId);

        await _mediator.Send(new DebitWalletCommand(message.UserId, message.OrderId, message.Amount));
    }
}