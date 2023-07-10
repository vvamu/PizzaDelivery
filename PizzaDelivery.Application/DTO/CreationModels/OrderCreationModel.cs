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
    public DateTime OrderDate { get; set; }
    public string Address { get; set; }
    public string PaymentType { get; set; }
    public string DeliveryType { get; set; }
    public string Comment { get; set; }

}
