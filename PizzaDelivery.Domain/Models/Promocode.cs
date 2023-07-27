using Microsoft.AspNetCore.Mvc.ModelBinding;
using PizzaDelivery.Models.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Domain.Models;

public class Promocode : BaseModel
{
    public string Value { get; set; }
    public int SalePercent { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool? Expired { get; set; } = false;

}
