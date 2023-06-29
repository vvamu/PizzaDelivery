using PizzaDelivery.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Models
{
    public class ShoppingCartItem : BaseModel
    {
        public ShoppingCartItem() 
        {
            Amount = 1;
        }

        [Required]
        public Guid ShoppingCartId { get; set; }

        public ShoppingCart ShoppingCart { get; set; }

        [Required]
        public Guid PizzaId { get; set; }

        public Pizza Pizza { get; set; }

        [Range(0, 20, ErrorMessage = "The field {0} must be greater than {1}.")]
        public int Amount { get; set; }

    }
}
