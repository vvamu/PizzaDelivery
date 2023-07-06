using DocumentFormat.OpenXml.Spreadsheet;
using PizzaDelivery.Application.Interfaces;
using PizzaDelivery.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaDelivery.Application.Helpers;
public class DefaultDbContent : IDefalutDbContent
{
    private readonly IAuthService _authService;
    private readonly IPizzaService _pizzaRepository;
    private readonly IPromocodeService _promocodeRepository;
    private readonly IShoppingCartService _shoppingCartRepository;
    private readonly IOrderService _orderRepository;

    public DefaultDbContent(IAuthService authService, IPizzaService pizzaRepository,
        IPromocodeService promocodeRepository, IShoppingCartService shoppingCartRepository,
        IOrderService orderRepository)
    {
        _authService = authService;
        _pizzaRepository = pizzaRepository;
        _promocodeRepository = promocodeRepository;
        _shoppingCartRepository = shoppingCartRepository;
        _orderRepository = orderRepository;
    }

    public async Task GenerateAll()
    {
        //await GenerateUsers();
        //await GeneratePizzas();
        //await GeneratePromocodes();
        await GenerateShoppingCartAndOrders();

    }
    public async Task GenerateUsers()
    {
        for (int i = 0; i < 15; i++)
        {
            var user = new RegisterUser
            {
                Email = "user" + (i + 1) + "@example.com",
                Username = "user" + (i + 1),
                Password = "Password*1" // Set the desired password
            };

            await _authService.RegisterAsync(user);
        }

        // Get the newly created users from the database
        var newUsers = await _authService.GetAllAsync();

        // Extract the user IDs for further use
        var userIds = newUsers.Select(u => u.Id).ToList();
    }
    public async Task GeneratePizzas()
    {
        // Get all existing pizzas from the database
        var existingPizzas = await _pizzaRepository.GetAllAsync();

        for (int i = 0; i < 15; i++)
        {
            var pizza = new PizzaCreationModel
            {
                Name = "Pizza " + (i + 1),
                Ingridients = "Ingredients for Pizza " + (i + 1),
                Price = 10.99m, // Set the desired price
                Desctiption = "Description for Pizza " + (i + 1),
            };

            await _pizzaRepository.CreateAsync(pizza);
        }

        // Get the newly created pizzas from the database
        var newPizzas = await _pizzaRepository.GetAllAsync();

        // Extract the pizza IDs for further use
        var pizzaIds = newPizzas.Except(existingPizzas).Select(p => p.Id).ToList();
    }
    public async Task GeneratePromocodes()
    {
        for (int i = 0; i < 15; i++)
        {
            var promocode = new PromocodeCreationModel
            {
                Value = "PROMO" + (i + 1),
                SalePercent = 20, // Set the desired sale percentage
                ExpireDate = DateTime.Now.AddDays(30) // Set the desired expiration date
            };

            await _promocodeRepository.CreateAsync(promocode);
        }
    }
    public async Task GenerateShoppingCartAndOrders()
    {
        var pizzas = _pizzaRepository.GetAllAsync().Result.ToArray();
        var users = _authService.GetAllAsync().Result.ToArray();


        foreach (var user in users)
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    Random random = new Random();
                    int randomIndex = random.Next(0, pizzas.Count());

                    var randomPizzaId = pizzas[randomIndex].Id;
                    await _shoppingCartRepository.UpdateItemInShoppingCartAsync(randomPizzaId, randomIndex);
                }
            }
            catch (Exception ex) { }
            // Create Orders
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    Random random = new Random();
                    int randomIndex = random.Next(0, pizzas.Count());

                    var pizzaId = pizzas[randomIndex].Id;
                    var userId = user.Id;

                    var order = new OrderCreationModel
                    {
                        OrderDate = DateTime.Now,
                        Address = "Address for User " + userId,
                        PaymentType = "CreditCard", // Set the desired payment type
                        DeliveryType = "HomeDelivery", // Set the desired delivery type
                        Comment = "Comment for Order " + (i + 1)
                    };

                    await _orderRepository.CreateAsync(order);
                }
            }
            catch (Exception ex) { }
        }
    }
}
