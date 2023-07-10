using Microsoft.AspNetCore.Mvc.ModelBinding;
using PizzaDelivery.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaDelivery.Application.Models;

public class PromocodeCreateModel
{
    public string Value { get; set; }
    public int SalePercent { get; set; }

    [DisplayName("Expire Date")]
    public DateTime ExpireDate { get; set; }
}
