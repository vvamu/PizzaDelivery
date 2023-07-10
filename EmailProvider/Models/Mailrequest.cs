using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailProvider.Models;

public class Mailrequest
{
    public string? ToEmail { get; set; } 
    public string? Subject { get; set; } 
    public string? TextBody { get; set; }
    public string? HtmlBody { get; set; }
    public IFormFile? File { get; set; }

}
