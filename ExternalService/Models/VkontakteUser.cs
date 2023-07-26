using PizzaDelivery.Application.DTO;
using PizzaDelivery.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExternalProvider.Models;

public class VkontakteUser :  ExternalUser
{

}

public class VkontakteUserResponse
{
    public List<VkontakteUser> Response { get; set; }

}
