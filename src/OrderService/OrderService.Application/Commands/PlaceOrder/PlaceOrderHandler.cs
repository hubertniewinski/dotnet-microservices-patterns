using MediatR;
using OrderService.Application.Interfaces;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Common;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Commands.PlaceOrder;

public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Guid>
{
    private readonly IOrderRepository _repository;
    private readonly IWalletClient _walletClient;

    public PlaceOrderHandler(IOrderRepository repository, IWalletClient walletClient) {
        _repository = repository;
        _walletClient = walletClient;
    }

    public async Task<Guid> Handle(PlaceOrderCommand cmd, CancellationToken ct)
    {
        var hasBalance = await _walletClient.CheckBalanceAsync(cmd.UserId, cmd.Amount, ct);
        if (!hasBalance)
        {
            throw new DomainException("Insufficient balance or wallet service unavailable");
        }

        var order = Order.Create(cmd.UserId, cmd.Amount);
        await _repository.SaveAsync(order, true, ct);
        return order.Id;
    }
}