using FluentValidation;
using PizzaDelivery.Domain.Models;
using PizzaDelivery.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaDelivery.Domain.Validators;

public class OrderValidator : AbstractValidator<OrderCreationModel>
{
    public OrderValidator()
    {
        RuleFor(x => x.OrderDate)
      .NotEmpty().WithMessage("Order Date is required.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .Length(10, 100).WithMessage("Address must be between 10 and 100 characters.")
            .Matches(@"^[a-zA-Z0-9\s.,-]*$").WithMessage("Invalid address format.");

        RuleFor(x => x.PaymentType)
            .NotEmpty().WithMessage("Payment Type is required.")
            .Must(BeValidPaymentType).WithMessage("Invalid Payment Type.");

        RuleFor(x => x.DeliveryType)
            .NotEmpty().WithMessage("Delivery Type is required.")
            .Must(BeValidDeliveryType).WithMessage("Invalid Delivery Type.");
    }

    private bool BeValidPaymentType(string paymentType)
    {
        return Enum.TryParse(typeof(PaymentType), paymentType, out _);
    }

    private bool BeValidDeliveryType(string deliveryType)
    {
        return Enum.TryParse(typeof(DeliveryType), deliveryType, out _);
    }
    private bool BeOrderStatus(string orderStatus)
    {
        return Enum.TryParse(typeof(OrderStatus), orderStatus, out _);
    }
}

