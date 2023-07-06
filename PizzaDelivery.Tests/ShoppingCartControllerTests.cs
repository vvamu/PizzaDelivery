using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PizzaDelivery.Application.Interfaces;
using PizzaDelivery.Domain.Models;
using PizzaDeliveryApi.Controllers;

namespace PizzaDelivery.Tests;

public class ShoppingCartControllerTests
{
    private readonly ShoppingCartController _controller;
    private readonly Mock<IShoppingCartService> _mockRepository;

    public ShoppingCartControllerTests()
    {
        _mockRepository = new Mock<IShoppingCartService>();
        _controller = new ShoppingCartController(Mock.Of<ILogger<ShoppingCartController>>(), _mockRepository.Object);
    }

    [Fact]
    public async Task GetShoppingCartAsync_ReturnsOkResult()
    {
        // Arrange
        var expectedShoppingCart = new ShoppingCart();
        _mockRepository.Setup(repo => repo.GetShoppingCartAsync()).ReturnsAsync(expectedShoppingCart);

        // Act
        var result = await _controller.GetShoppingCartAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualShoppingCart = Assert.IsType<ShoppingCart>(okResult.Value);
        Assert.Equal(expectedShoppingCart, actualShoppingCart);
    }

    [Fact]
    public async Task GetAllItems_ReturnsOkResult()
    {
        // Arrange
        var expectedShoppingCartItems = new List<ShoppingCartItem>();
        _mockRepository.Setup(repo => repo.GetAllShoppingCartItemsAsync()).ReturnsAsync(expectedShoppingCartItems);

        // Act
        var result = await _controller.GetAllShoppingCartItemsAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualShoppingCartItems = Assert.IsType<List<ShoppingCartItem>>(okResult.Value);
        Assert.Equal(expectedShoppingCartItems, actualShoppingCartItems);
    }

    [Fact]
    public async Task AddOneToShoppingCartAsync_ReturnsOkResult()
    {
        // Arrange
        var pizzaId = Guid.NewGuid();
        var expectedShoppingCartItem = new ShoppingCartItem();
        _mockRepository.Setup(repo => repo.AddOneToShoppingCartAsync(pizzaId)).ReturnsAsync(expectedShoppingCartItem);

        // Act
        var result = await _controller.AddOneToShoppingCartAsync(pizzaId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualShoppingCartItem = Assert.IsType<ShoppingCartItem>(okResult.Value);
        Assert.Equal(expectedShoppingCartItem, actualShoppingCartItem);
    }

    [Fact]
    public async Task RemoveOneFromShoppingCartAsync_ReturnsOkResult()
    {
        // Arrange
        var shoppingCartItemId = Guid.NewGuid();
        var expectedShoppingCartItem = new ShoppingCartItem();
        _mockRepository.Setup(repo => repo.RemoveOneFromShoppingCartAsync(shoppingCartItemId)).ReturnsAsync(expectedShoppingCartItem);

        // Act
        var result = await _controller.RemoveOneFromShoppingCartAsync(shoppingCartItemId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualShoppingCartItem = Assert.IsType<ShoppingCartItem>(okResult.Value);
        Assert.Equal(expectedShoppingCartItem, actualShoppingCartItem);
    }

    [Fact]
    public async Task UpdateItemInShoppingCartAsync_ReturnsOkResult()
    {
        // Arrange
        var pizzaId = Guid.NewGuid();
        var amount = 2;
        var expectedShoppingCartItem = new ShoppingCartItem();
        _mockRepository.Setup(repo => repo.UpdateItemInShoppingCartAsync(pizzaId, amount)).ReturnsAsync(expectedShoppingCartItem);

        // Act
        var result = await _controller.UpdateItemInShoppingCartAsync(pizzaId, amount);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualShoppingCartItem = Assert.IsType<ShoppingCartItem>(okResult.Value);
        Assert.Equal(expectedShoppingCartItem, actualShoppingCartItem);
    }

    [Fact]
    public async Task ClearCartAsync_ReturnsOkResult()
    {
        // Arrange
        var expectedShoppingCartItems = new List<ShoppingCartItem>();
        _mockRepository.Setup(repo => repo.ClearCartAsync()).ReturnsAsync(expectedShoppingCartItems);

        // Act
        var result = await _controller.ClearCartAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualShoppingCartItems = Assert.IsType<List<ShoppingCartItem>>(okResult.Value);
        Assert.Equal(expectedShoppingCartItems, actualShoppingCartItems);
    }
}