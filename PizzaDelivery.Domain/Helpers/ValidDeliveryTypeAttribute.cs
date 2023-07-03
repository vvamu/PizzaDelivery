using PizzaDelivery.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Helpers;

public class ValidDeliveryTypeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null)
        {
            string orderStatus = value.ToString();

            if (!Enum.IsDefined(typeof(DeliveryType), orderStatus))
            {
                return new ValidationResult("Invalid Delivery Type");
            }
        }

        return ValidationResult.Success;
    }

}