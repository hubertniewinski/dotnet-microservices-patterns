using MediatR;
using WalletService.Application.Interfaces;
using WalletService.Domain.Common;
using WalletService.Domain.Interfaces;

namespace WalletService.Application.Commands.DebitWallet;

public class DebitWalletHandler : IRequestHandler<DebitWalletCommand>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IIdempotencyRepository _idempotencyRepository;

    public DebitWalletHandler(IWalletRepository walletRepository, IIdempotencyRepository idempotencyRepository)
    {
        _walletRepository = walletRepository;
        _idempotencyRepository = idempotencyRepository;
    }
    
    public async Task Handle(DebitWalletCommand request, CancellationToken cancellationToken)
    {
        var existing = await _idempotencyRepository.GetAsync(request.IdempotencyKey, cancellationToken);
        if (existing is not null)
        {
            return;
        }

        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new DomainException($"Wallet not found for user {request.UserId}");

        wallet.Debit(request.Amount, request.OrderId);

        //TO DO: atomic transaction
        await _walletRepository.SaveAsync(wallet, ct: cancellationToken);
        await _idempotencyRepository.StoreAsync(request.IdempotencyKey, "debited", cancellationToken);
    }
}