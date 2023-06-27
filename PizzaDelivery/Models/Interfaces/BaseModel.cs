using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Models.Interfaces
{
    public abstract class BaseModel
    {
        [Key]
        public virtual Guid Id { get; set; }
    }
}
