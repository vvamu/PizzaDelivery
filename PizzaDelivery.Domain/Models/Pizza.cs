using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.Domain.Models;

public class Pizza : BaseModel
{

    public string Name { get; set; }
    public string Ingridients { get; set; }

    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
    public string Description { get; set; } = "";

    public string? ImagePath { get; set; }
    public string? ImageMime { get; set; }

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? ImageFile { get; set; }

}
