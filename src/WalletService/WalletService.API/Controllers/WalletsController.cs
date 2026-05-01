using MediatR;
using Microsoft.AspNetCore.Mvc;
using WalletService.API.Requests;
using WalletService.Application.Commands.DebitWallet;
using WalletService.Application.Commands.SeedWallet;

namespace WalletService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WalletsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("debit")]
    public async Task<IActionResult> DebitAsync([FromBody] DebitWalletRequest request, CancellationToken ct)
    {
        await _mediator.Send(new DebitWalletCommand(request.UserId, request.OrderId, request.Amount), ct);

        return Ok();
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedAsync([FromBody] SeedWalletRequest request,
        CancellationToken ct)
    {
        await _mediator.Send(new SeedWalletCommand(request.UserId, request.InitialBalance), ct);

        return Ok();
    }

    [HttpGet("chaos")]
    public async Task<IActionResult> ChaosAsync()
    {
        await Task.Delay(2000);
        return StatusCode(500);
    }
}