using DocumentFormat.OpenXml.Wordprocessing;
using PizzaDelivery.Helpers;
using PizzaDelivery.Models.Enums;
using PizzaDelivery.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.DTO;

public class OrderDTO : BaseModel
{
    public decimal TotalPrice { get; set; }
    public string OrderItems { get; set; }


    [DisplayName("Order Date")]
    public DateTime OrderDate { get; set; }
    public string Address { get; set; }

    [Display(Name = "Payment Type")]
    public string PaymentType { get; set; }

    [Display(Name = "Delivery Type")]
    public string DeliveryType { get; set; }
    public string Comment { get; set; }
}

