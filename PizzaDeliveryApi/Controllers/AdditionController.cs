using EmailProvider.Models;
using EmailProvider.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmailProvider;
using MailKit;
using EmailProvider.Interfaces;

namespace PizzaDeliveryApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AdditionController : ControllerBase
{
    private readonly EmailProvider.Interfaces.IMailService _mailService;

    public AdditionController(EmailProvider.Interfaces.IMailService emailSender)
    {
        _mailService = emailSender;
    }

    [HttpPost("Email",Name = "SendEmail")]
    public async Task<ActionResult<Mailrequest>> SendEmailAsync([FromForm] Mailrequest mailrequest)
    {
        
        try
        {
            await _mailService.SendEmailAsync(mailrequest);
            return Ok(mailrequest);
        }
        catch (Exception ex)
        {
            throw;
            return BadRequest(ex);
        }
    }

    private string GetHtmlcontent()
    {
        string Response = "<div style=\"width:100%;background-color:lightblue;text-align:center;margin:10px\">";
        Response += "<h1>Welcome to Nihira Techiees</h1>";
        Response += "<img src=\"https://yt3.googleusercontent.com/v5hyLB4am6E0GZ3y-JXVCxT9g8157eSeNggTZKkWRSfq_B12sCCiZmRhZ4JmRop-nMA18D2IPw=s176-c-k-c0x00ffffff-no-rj\" />";
        Response += "<h2>Thanks for subscribed us</h2>";
        Response += "<a href=\"https://www.youtube.com/channel/UCsbmVmB_or8sVLLEq4XhE_A/join\">Please join membership by click the link</a>";
        Response += "<div><h1> Contact us : nihiratechiees@gmail.com</h1></div>";
        Response += "</div>";
        return Response;
    }


}
