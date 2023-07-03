using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.Domain.Models;
using PizzaDelivery.DomainRealize.Interfaces;
using PizzaDelivery.Services;
using PizzaDelivery.ViewModels;
using System.Diagnostics;

namespace PizzaDelivery.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private IRepository<Pizza> pizzaRepository;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        //this.pizzaRepository= pizzaRepository;
    }

    public IActionResult Index()
    {
        //var a = pizzaRepository.GetAllAsync().Result.ToList();
        //ViewBag.pizzas = a.ToList();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult CreateNew()
    {
        var a = new Pizza()
        {
            Name = "Margarita",
            Ingridients = "Dought Tomatoes Cheese",
            Price = 10,
            Desctiption = "Pizza without meat!"
        };
        pizzaRepository.CreateAsync(a);
        var b = pizzaRepository.GetAllAsync();
        return RedirectToAction("Index");
    }
}