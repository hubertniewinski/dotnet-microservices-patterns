using MediatR;
using WalletService.Domain.Common;
using WalletService.Domain.Interfaces;

namespace WalletService.Application.Commands.DebitWallet;

public class DebitWalletHandler : IRequestHandler<DebitWalletCommand>
{
    private readonly IWalletRepository _walletRepository;

    public DebitWalletHandler(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }
    
    public async Task Handle(DebitWalletCommand request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new DomainException($"Wallet not found for user {request.UserId}");

        wallet.Debit(request.Amount, request.OrderId);

        await _walletRepository.SaveAsync(wallet, cancellationToken);
    }
}