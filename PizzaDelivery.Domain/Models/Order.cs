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
    public Order()
    {
        ShoppingCartItems = new List<ShoppingCartItem>();
        OrderDate = DateTime.Now;
    }

    [BindNever]
    [DisplayName("Total Price")]
    public decimal TotalPrice { get; set; }

    [DisplayName("Order Date")]
    public DateTime OrderDate { get; set; }

    [StringLength(100,MinimumLength = 10)]
    //[ValidateAddress(ErrorMessage = "Invalid address")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,-]*$", ErrorMessage = "Invalid address format")]
    public string Address { get; set; }

    [Display(Name = "Payment Type")]
    [EnumDataType(typeof(PaymentType))]

    public string PaymentType { get; set; }

    [Display(Name = "Delivery Type")]
    [EnumDataType(typeof(DeliveryType))]
    public string DeliveryType { get; set; }

    [Display(Name = "Order Status")]
    [ValidOrderStatus]
    [EnumDataType(typeof(OrderStatus))]
    public string OrderStatus { get; set; }
    public string Comment { get; set; }

    //---------------------------------
    public string UserId { get; set; }

    public ApplicationUser User { get; set; }

    public Guid? PromocodeId { get; set; }
    public Promocode? Promocode { get; set; }

    public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
}
