using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.Domain.Models;

public class Pizza : BaseModel
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
    public string Description { get; set; }

    [DataType(DataType.Text)]
    public string? ImagePath { get; set; }

    [DataType(DataType.Text)]
    public string? ImageMime { get; set; }

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? ImageFile { get; set; }
    //public bool Deleted { get; set; }

}
