using Microsoft.AspNetCore.Mvc.ModelBinding;
using PizzaDelivery.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaDelivery.Application.Models;

public class PromocodeCreationModel
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
}
