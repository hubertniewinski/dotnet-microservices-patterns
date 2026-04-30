using FluentValidation;

namespace WalletService.Application.Commands.DebitWallet;

public class DebitWalletValidator : AbstractValidator<DebitWalletCommand>
{
    public DebitWalletValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(100_000);
    }
}