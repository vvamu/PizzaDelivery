using GoogleMaps.LocationServices;
using PizzaDelivery.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Helpers;

public class ValidateAddressAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string address = value as string;
        if (string.IsNullOrEmpty(address))
            return new ValidationResult("Invalid Address."); // Allow empty or null address
      
        var locationService = new GoogleLocationService();
        var point = locationService.GetLatLongFromAddress(address);

        // If the latitude and longitude are obtained, the address is valid
        if(point.Latitude != 0 && point.Longitude != 0)
            return ValidationResult.Success;

        return new ValidationResult("Invalid Address");

    }
}