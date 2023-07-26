using Microsoft.AspNetCore.Http;

namespace EmailProvider.Models;

public class Mailrequest
{
    public string? ToEmail { get; set; }
    public string? Subject { get; set; }
    public string? TextBody { get; set; }
    public string? HtmlBody { get; set; }
    public IFormFile? File { get; set; }

}
