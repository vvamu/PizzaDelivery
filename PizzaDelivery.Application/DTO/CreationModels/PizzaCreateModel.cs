using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.Application.Models;

public class PizzaCreateModel
{
    public string Name { get; set; }
    public string Ingridients { get; set; }
    public decimal Price { get; set; }
    public string Desctiption { get; set; }

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? ImageFile { get; set; }
}
