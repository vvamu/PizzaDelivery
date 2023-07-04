using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaDelivery.Application.Models;

public class PizzaCreationModel
{
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "The field Name should only include letters and number.")]
    [DataType(DataType.Text)]
    [Required]
    public string Name { get; set; }

    [StringLength(255, MinimumLength = 2)]
    [DataType(DataType.MultilineText)]
    [Required]
    public string Ingridients { get; set; }

    [Range(0, 1000)]
    [DataType(DataType.Currency)]
    [Required]
    public decimal Price { get; set; }

    [StringLength(255, MinimumLength = 2)]
    [DataType(DataType.MultilineText)]
    [Required]
    public string Desctiption { get; set; }

    [DataType(DataType.Text)]
    public string? ImageUrl { get; set; }

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? ImageFile { get; set; }
}
