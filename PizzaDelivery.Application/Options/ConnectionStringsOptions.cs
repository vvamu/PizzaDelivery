using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaDelivery.Application.Options;

public class ConnectionStringsOptions
{
    public const string OptionName = "ConnectionStrings";
    public string DefaultConnection { get; set; }
    public string SqliteConnection { get; set; }
}
