using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Models.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Models
{
    public class ShoppingCart : BaseModel
    {
        public ShoppingCart() 
        {
            ShoopingCartPizzas = new List<ShoopingCartPizzas>();
        }
        public List<ShoopingCartPizzas> ShoopingCartPizzas { get; set; }

    }
}
