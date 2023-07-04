using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Helpers;
using PizzaDelivery.Models.Enums;
using PizzaDelivery.Models.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Application.Models;

public class OrderCreationModel
{
    [ScaffoldColumn(false)]
    [DisplayName("Order Date")]
    public DateTime OrderDate { get; set; }

    [Required(ErrorMessage = "Please enter your address")]
    [StringLength(100, MinimumLength = 10)]
    //[ValidateAddress(ErrorMessage = "Invalid address")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,-]*$", ErrorMessage = "Invalid address format")]
    public string Address { get; set; }

    [Display(Name = "Payment Type")]
    [Required(ErrorMessage = "Please choose type of payment")]
    [ValidPaymentType]
    [EnumDataType(typeof(PaymentType))]
    public string PaymentType { get; set; }

    [Display(Name = "Delivery Type")]
    [Required(ErrorMessage = "Please choose type of delivery")]
    [ValidDeliveryType]
    [EnumDataType(typeof(DeliveryType))]

    public string DeliveryType { get; set; }

    [DefaultValue("")]
    public string Comment { get; set; }


}
