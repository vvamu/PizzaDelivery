using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.Application.Interfaces;
using PizzaDelivery.Domain.Models;
using PizzaDelivery.Services;
using PizzaDelivery.ViewModels;
using System.Diagnostics;

namespace PizzaDelivery.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private IPizzaRepository pizzaRepository;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        //this.pizzaRepository= pizzaRepository;
    }

    public IActionResult Index()
    {

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


}