using MediatR;
using WalletService.Domain.Aggregates;
using WalletService.Domain.Interfaces;

namespace WalletService.Application.Commands.SeedWallet;

public class SeedWalletHandler : IRequestHandler<SeedWalletCommand>
{
    private readonly IWalletRepository _repository;

    public SeedWalletHandler(IWalletRepository repository) => _repository = repository;

    public async Task Handle(SeedWalletCommand cmd, CancellationToken ct)
    {
        var wallet = Wallet.Create(cmd.UserId, cmd.InitialBalance);
        await _repository.SaveAsync(wallet, isNew: true, ct);
    }
}