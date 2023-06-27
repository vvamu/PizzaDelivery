using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Models.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Models
{
    public class Order : BaseModel
    {

        [Display(Name = "Payment Type")]
        [Required(ErrorMessage = "Please choose type of payment")]
        public string PaymentType { get; set; }

        [BindNever]
        [ScaffoldColumn(false)]
        [DisplayName("Order Total")]
        [Precision(18, 2)]
        public decimal OrderTotal { get; set; }

        [BindNever]
        [ScaffoldColumn(false)]
        [DisplayName("Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Please enter your address")]
        [StringLength(100)]
        public string Address { get; set; }


        [Display(Name = "Delivery Type")]
        [Required(ErrorMessage = "Please choose type of delivery")]
        public string DeliveryType { get; set; }


        public string? Comment { get; set; }

        public Promocode? Promocode { get; set; }

        //---------------------------------
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

    }
}
