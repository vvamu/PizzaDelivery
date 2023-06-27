using PizzaDelivery.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Models
{
    public class ShoopingCartPizzas : BaseModel
    {
        public ShoopingCartPizzas() 
        {
            Amount = 0;
        }

        [Required]
        public Guid ShoppingCartId { get; set; }

        public ShoppingCart ShoppingCart { get; set; }

        [Required]
        public Guid PizzaId { get; set; }

        public Pizza Pizza { get; set; }
        public int Amount { get; set; }

    }
}
