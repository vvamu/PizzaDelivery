using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Models;

public class Pizzas
{
    public Pizzas()
    {
        //PizzaIngredients = new HashSet<PizzaIngredients>();
        //Reviews = new HashSet<Reviews>();
    }

    public int Id { get; set; }

    [StringLength(100, MinimumLength = 2)]
    [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "The field Name should only include letters and number.")]
    [DataType(DataType.Text)]
    [Required]
    public string Name { get; set; }

    [Range(0, 1000)]
    [DataType(DataType.Currency)]
    [Required]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    [StringLength(255, MinimumLength = 2)]
    [DataType(DataType.MultilineText)]
    [Required]
    public string Ingridients { get; set; }

    public string? ImagePath { get; set; }

}
