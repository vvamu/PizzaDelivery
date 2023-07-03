using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Helpers;
using PizzaDelivery.Models.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Application.Models;

public class CreatedOrder
{
    [BindNever]
    [ScaffoldColumn(false)]
    [DisplayName("Pricead Total")]
    [Precision(18, 2)]
    public decimal TotalPrice { get; set; }

    [ScaffoldColumn(false)]
    [DisplayName("Order Date")]
    public DateTime OrderDate { get; set; }

    [Required(ErrorMessage = "Please enter your address")]
    [StringLength(100)]
    public string Address { get; set; }

    [Display(Name = "Payment Type")]
    [Required(ErrorMessage = "Please choose type of payment")]
    [ValidPaymentType]
    public string PaymentType { get; set; }

    [Display(Name = "Delivery Type")]
    [Required(ErrorMessage = "Please choose type of delivery")]
    [ValidDeliveryType]
    public string DeliveryType { get; set; }

    [Display(Name = "Order Status")]
    [ValidOrderStatus]
    public string OrderStatus { get; set; }

    [DefaultValue("")]
    public string Comment { get; set; }


}
