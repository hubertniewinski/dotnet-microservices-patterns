using FluentValidation;

namespace OrderService.Application.Commands.PlaceOrder;

public class PlaceOrderValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(100_000);
    }
}