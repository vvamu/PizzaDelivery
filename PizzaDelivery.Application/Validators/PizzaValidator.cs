using FluentValidation;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Domain.Validators;

public class PizzaValidator : AbstractValidator<PizzaCreateModel>
{
    public PizzaValidator()
    {
        RuleFor(pizza => pizza.Name)
            .NotEmpty().WithMessage("The Name field is required.")
            .Length(2, 100).WithMessage("The Name field must be between 2 and 100 characters.")
            .Matches("([a-zA-Z0-9 .&'-]+)").WithMessage("The Name field should only include letters and numbers.");

        RuleFor(pizza => pizza.Ingridients)
            .NotEmpty().WithMessage("The Ingredients field is required.")
            .Length(2, 255).WithMessage("The Ingredients field must be between 2 and 255 characters.");

        RuleFor(pizza => pizza.Price)
            .NotEmpty().WithMessage("The Price field is required.")
            .InclusiveBetween(0, 1000).WithMessage("The Price field must be between 0 and 1000.");

        RuleFor(pizza => pizza.Desctiption)
            .NotEmpty().WithMessage("The Description field is required.")
            .Length(2, 255).WithMessage("The Description field must be between 2 and 255 characters.");

    }
}
