using Microsoft.AspNetCore.Mvc.ModelBinding;
using PizzaDelivery.Domain.Models.User;
using PizzaDelivery.Helpers;
using PizzaDelivery.Models.Enums;
using PizzaDelivery.Models.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Domain.Models;

public class Order : BaseModel
{
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public string Address { get; set; }
    public string PaymentType { get; set; }
    public string DeliveryType { get; set; }
    public string OrderStatus { get; set; } = "NotDelivered";
    public string Comment { get; set; } = "";

    //---------------------------------
    public string UserId { get; set; }

    public ApplicationUser User { get; set; }

    public Guid? PromocodeId { get; set; }
    public Promocode? Promocode { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
