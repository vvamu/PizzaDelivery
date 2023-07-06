using Microsoft.AspNetCore.Mvc.ModelBinding;
using PizzaDelivery.Models.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Domain.Models;

public class Promocode : BaseModel
{
    [StringLength(20, MinimumLength = 5)]
    [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "The field Name should only include letters and number.")]
    [DataType(DataType.Text)]
    [Required]
    public string Value { get; set; }

    [Required]
    [Range(1, 100,
    ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int SalePercent { get; set; }

    [BindNever]
    [ScaffoldColumn(false)]
    [DisplayName("Expire Date")]
    [Required]
    public DateTime ExpireDate { get; set; }
    public bool Expired { get; set; }

}
