using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Models;

public class Pizza : BaseModel
{
    public Pizza()
    {
        if(ImagePath == string.Empty|| ImagePath == null)
        {
            ImagePath = "E:\\education\\asp\\PizzaDelivery\\PizzaDelivery\\wwwroot\\images\\default.jpg";
        }
    }

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
    [Precision(18, 2)]
    public decimal Price { get; set; }


    [StringLength(255, MinimumLength = 2)]
    [DataType(DataType.MultilineText)]
    [Required]
    public string Desctiption { get; set; }

    [DataType(DataType.Text)]
    public string? ImagePath { get; set; }

}
