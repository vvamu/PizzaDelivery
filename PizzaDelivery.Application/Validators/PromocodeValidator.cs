using FluentValidation;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Domain.Validators;

public class PromocodeValidator : AbstractValidator<PromocodeCreateModel>
{
    public PromocodeValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required.")
            .Length(5, 20).WithMessage("Value must be between 5 and 20 characters.")
            .Matches("^[a-zA-Z0-9 .&'-]+$").WithMessage("Value should only include letters and numbers.");

        RuleFor(x => x.SalePercent)
            .NotEmpty().WithMessage("SalePercent is required.")
            .InclusiveBetween(1, 100).WithMessage("SalePercent must be between 1 and 100.");

        RuleFor(x => x.ExpireDate)
            .NotEmpty().WithMessage("ExpireDate is required.")
            .GreaterThan(DateTime.Now).WithMessage("ExpireDate must be a future date.");

    }
}
