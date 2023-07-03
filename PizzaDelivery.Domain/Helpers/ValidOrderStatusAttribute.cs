using PizzaDelivery.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Helpers;

public class ValidOrderStatusAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null)
        {
            string orderStatus = value.ToString();

            if (!Enum.IsDefined(typeof(OrderStatus), orderStatus))
            {
                return new ValidationResult("Invalid Order Status");
            }
        }

        return ValidationResult.Success;
    }

}
