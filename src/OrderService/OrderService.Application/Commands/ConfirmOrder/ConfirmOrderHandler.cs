using MediatR;
using OrderService.Domain.Common;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Commands.ConfirmOrder;

public class ConfirmOrderHandler : IRequestHandler<ConfirmOrderCommand>
{
    private readonly IOrderRepository _repository;

    public ConfirmOrderHandler(IOrderRepository repository) => _repository = repository;

    public async Task Handle(ConfirmOrderCommand cmd, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(cmd.OrderId, ct) 
            ?? throw new DomainException($"Order not found {cmd.OrderId}");

        order.Confirm();
        await _repository.SaveAsync(order, ct: ct);
    }
}