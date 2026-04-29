using MediatR;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Commands.PlaceOrder;

public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Guid>
{
    private readonly IOrderRepository _repository;

    public PlaceOrderHandler(IOrderRepository repository) => _repository = repository;

    public async Task<Guid> Handle(PlaceOrderCommand cmd, CancellationToken ct)
    {
        var order = Order.Create(cmd.UserId, cmd.Amount);
        await _repository.SaveAsync(order, ct);
        return order.Id;
    }
}