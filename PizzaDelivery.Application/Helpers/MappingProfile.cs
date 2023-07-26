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
        CreateMap<IdentityUser, ApplicationUser>().ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UserRegister, ApplicationUser>().ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UserLogin, ApplicationUser>().ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ShoppingCart, ShoppingCartDTO>().ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ShoppingCartItem, ShoppingCartItemDTO>().ReverseMap()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ShoppingCartItem, OrderItem>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<PizzaCreateModel, Pizza>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ApplicationUser, UserDTO>().ReverseMap().ForMember(dest => dest.OwnHashedPassword, opt => opt.MapFrom(src => src.Password));
        CreateMap<OrderCreationModel, Order>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<PromocodeCreateModel, Promocode>().ReverseMap().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<Order, OrderDTO>().ReverseMap()
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


