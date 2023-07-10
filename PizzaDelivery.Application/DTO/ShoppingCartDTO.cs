using PizzaDelivery.Domain.Models.User;
using PizzaDelivery.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaDelivery.Application.DTO;

public class ShoppingCartDTO
{
    public decimal TotalPrice { get; set; }
    public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }

    public string UserId { get; set; }

}
