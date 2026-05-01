using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.Requests;
using OrderService.Application.Commands.PlaceOrder;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> PlaceOrderAsync([FromBody] PlaceOrderRequest request, CancellationToken ct)
    {
        var orderId = await _mediator.Send(new PlaceOrderCommand(request.UserId, request.Amount), ct);

        return Created($"api/orders/{orderId}", new { id = orderId });
    }
}