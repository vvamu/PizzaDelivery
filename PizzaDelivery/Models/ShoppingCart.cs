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
            ShoppingCartItems = new List<ShoppingCartItem>();
        }

        public decimal TotalPrice { get; set; }
        public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Promocode? Promocode { get; set; }

    }
}
