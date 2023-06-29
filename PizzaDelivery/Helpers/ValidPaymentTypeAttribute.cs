using PizzaDelivery.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Helpers;

public class ValidPaymentTypeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null)
        {
            string orderStatus = value.ToString();

            if (!Enum.IsDefined(typeof(PaymentType), orderStatus))
            {
                return new ValidationResult("Invalid Payment Type");
            }
        }

        return ValidationResult.Success;
    }

}