using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Domain.Models;
using PizzaDelivery.Helpers;
using PizzaDelivery.Models.Enums;
using System.Net;
using Microsoft.AspNetCore.Identity;
using PizzaDelivery.Domain.Models.User;
using PizzaDelivery.Application.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using PizzaDelivery.Application.Services.Interfaces;
using AutoMapper;
using PizzaDelivery.Application.DTO;
using PizzaDelivery.Domain.Validators;

namespace PizzaDelivery.Application.Services.Implementation;

public class OrderService : AbstractTransactionService, IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;


    public OrderService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, IMapper mapper) : base(context)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }
    private ApplicationUser CurrentUser
    {
        get
        {
            var username = _signInManager.Context.User.Identity.Name;
            var current_user = _userManager.Users.FirstOrDefault(x => x.UserName == username);
            return current_user == null ? throw new Exception("Unauthorized") : current_user;
        }
    }

    public PagedList<OrderDTO> GetAllAsync(QueryStringParameters ownerParameters)
    {
        var orders = _context.Orders.OrderByDescending(o => o.OrderStatus == "NotDelivered").ThenByDescending(x => x.OrderDate);
        IQueryable<OrderDTO> ordersDTO = orders.Select(x=> _mapper.Map<OrderDTO>(x));

        return PagedList<OrderDTO>.ToPagedList(ordersDTO, ownerParameters.PageNumber, ownerParameters.PageSize);
    }

    public PagedList<OrderDTO> GetAllByUserAsync(QueryStringParameters ownerParameters)
    {
        var orders = _context.Orders.Where(x => x.UserId == CurrentUser.Id).
            OrderByDescending(o => o.OrderStatus == "NotDelivered").ThenByDescending(x => x.OrderDate);
        var ordersDTO = orders.Select(x => _mapper.Map<OrderDTO>(x));
        return PagedList<OrderDTO>.ToPagedList(ordersDTO, ownerParameters.PageNumber, ownerParameters.PageSize);
    }

    public async Task<OrderDTO> GetAsync(Guid orderId)
    {
        var order = await _context.Orders.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == orderId);
        var userOrders = await GetAllByUserAsync();
        if (_signInManager.Context.User.IsInRole("User") && userOrders.Any(o => o.Id == orderId))
        {
            order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            return _mapper.Map<OrderDTO>(order);
        }
        throw new Exception("No Access");
    }

    public async Task<OrderDTO> CreateAsync(OrderCreationModel item)
    {
        var validator = new OrderValidator();
        var validationResult = await validator.ValidateAsync(item);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;
            var errorsString = String.Concat(errors);
            throw new Exception(errorsString);
        }

        var dbShoppingCart = await _context.ShoppingCart.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Id == CurrentUser.Id);
        var totalPrice = dbShoppingCart.TotalPrice;
        if (totalPrice == 0) throw new Exception("Add items to create order");

        var correctItem = _mapper.Map<Order>(item);
        correctItem.TotalPrice = totalPrice;
        correctItem.UserId = CurrentUser.Id;

        await BeginTransactionAsync();
        try
        {
            await _context.Orders.AddAsync(correctItem);
            await _context.SaveChangesAsync();

            var shoppingCart = await _context.ShoppingCart.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);


            if (shoppingCart != null)
            {
                var cartItems = await _context.ShoopingCartPizzas
                    .Where(x => x.ShoppingCartId == shoppingCart.Id)
                    .ToListAsync();
                List<OrderItem> orderItems = new();
                foreach (var cartItem in cartItems)
                {
                    var orderItem = _mapper.Map<OrderItem>(cartItem);
                    orderItem.OrderId = correctItem.Id;

                    orderItems.Add(orderItem);
                    correctItem.OrderItems.Add(orderItem);
                }
                await _context.OrderItems.AddRangeAsync(orderItems);
                _context.ShoopingCartPizzas.RemoveRange(cartItems);
            }
            await _context.SaveChangesAsync();
        }
        catch(Exception ex) {await RollbackAsync(); }
        await CommitAsync();

        return _mapper.Map<OrderDTO>(correctItem);
    }
    public async Task<ICollection<OrderItem>> GetAllOrderItems()
    {
        return await _context.OrderItems.Include(x => x.Order).Include(x => x.Pizza).ToListAsync();
    }


    private async Task<ICollection<Order>> GetAllAsync()
    {
        var notDeliveredOrderStatus = Enum.GetName(typeof(OrderStatus), 1);
        return await _context.Orders.OrderByDescending(o => o.OrderStatus == "NotDelivered").ThenByDescending(x => x.OrderDate).ToListAsync();
    }
    private async Task<ICollection<Order>> GetNotDeliveredOrdersAsync()
    {
        return await _context.Orders.OrderByDescending(x => x.OrderDate).Where(o => o.OrderStatus == "NotDelivered").ToListAsync();
    }

    private async Task<ICollection<Order>> GetAllByUserAsync(string userId = null)
    {
        if (string.IsNullOrEmpty(userId))
        {
            var username = _signInManager.Context.User.Identity.Name;
            var current_user = await _userManager.FindByNameAsync(username);
            userId = current_user.Id;
        }
        return await _context.Orders.Include(x => x.User).Where(x => x.User.Id == userId).ToListAsync();
    }
    private async Task<Order> GetOrderAsync(Guid orderId)
    {
        var order = await _context.Orders.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == orderId);
        var userOrders = await GetAllByUserAsync();
        if (_signInManager.Context.User.IsInRole("User") && userOrders.Any(o => o.Id == orderId))
        {
            order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            return order;
        }
        throw new Exception("No Access");
    }

    public async Task<OrderDTO> AddPromocodeToOrder(string promocodeValue, Guid orderId = default)
    {
        var order = await GetOrderAsync(orderId);
        if (!(await GetNotDeliveredOrdersAsync()).Any(x => x.Id == order.Id)) throw new Exception("Order was delivered");
        var dbPromocode = await CheckCorrectPromocode(promocodeValue);
        if (order.PromocodeId != default)
        {
            var existingPromoCode = await _context.Promocodes.FindAsync(order.PromocodeId);
            var total2 = order.TotalPrice / (decimal)(existingPromoCode.SalePercent / (double)100);
            order.TotalPrice = total2;
        }
        order.TotalPrice = (decimal)((double)order.TotalPrice * (100 - (double)dbPromocode.SalePercent) / 100);
        order.PromocodeId = dbPromocode.Id;
        try
        {
            _context.Orders.Update(order);
        }
        catch { }
        await _context.SaveChangesAsync();
        var orderDTO = await GetAsync(orderId);
        return orderDTO;
    }
    private async Task<Promocode> CheckCorrectPromocode(string promocodeValue)
    {
        var promocode = await _context.Promocodes.FirstOrDefaultAsync(x => x.Value == promocodeValue);
        return promocode == null ? throw new Exception("No such promocode in database") : promocode;
    }

}

