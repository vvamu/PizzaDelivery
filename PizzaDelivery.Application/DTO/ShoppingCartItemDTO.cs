using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaDelivery.Application.DTO;

public class ShoppingCartItemDTO
{
    public Guid OrderId { get; set; }
    public Guid PizzaId { get; set; }
    public int Amount { get; set; }
}
