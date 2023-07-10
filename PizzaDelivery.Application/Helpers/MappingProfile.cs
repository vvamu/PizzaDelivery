using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PizzaDelivery.Application.DTO;
using PizzaDelivery.Domain.Models;
using PizzaDelivery.Domain.Models.User;

namespace PizzaDelivery.Application.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<IdentityUser, ApplicationUser>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UserRegister, ApplicationUser>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UserLogin, ApplicationUser>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ShoppingCart, ShoppingCartDTO>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ShoppingCartItem, ShoppingCartItemDTO>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ApplicationUser, UserDTO>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.OwnHashedPassword));
        CreateMap<Order, OrderDTO>()
           .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => GetOrderItemsString(src.OrderItems)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


    }
    private static string GetOrderItemsString(ICollection<OrderItem> orderItems)
    {
        var orderItemsString = orderItems
            .Select(oi => $"{oi.Pizza.Name} - {oi.Amount}")
            .ToList();

        return string.Join(", ", orderItemsString);
    }
}


