using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PizzaDelivery.Application.Helpers;
public class Error
{
    public Guid ErrorId { get; set; } = Guid.NewGuid();
    public string Message { get; set; } = string.Empty;
    public string ExceptionType { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; }
    public string? StackTrace { get; set; } = string.Empty;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}