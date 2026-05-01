using MediatR;
using OrderService.Domain.Common;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Commands.FailOrder;

public class FailOrderHandler : IRequestHandler<FailOrderCommand>
{
    private readonly IOrderRepository _repository;

    public FailOrderHandler(IOrderRepository repository) => _repository = repository;

    public async Task Handle(FailOrderCommand cmd, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(cmd.OrderId, ct) ?? throw new DomainException($"Order not found {cmd.OrderId}");

        order.Fail();
        await _repository.SaveAsync(order, ct: ct);
    }
}