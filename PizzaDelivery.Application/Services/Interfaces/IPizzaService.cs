using PizzaDelivery.Application.Helpers;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Services.Interfaces;

public interface IPizzaService
{
    public PagedList<Pizza> GetAllAsync(QueryStringParameters ownerParameters);
    Task<Pizza?> GetAsync(Guid id); // получение одного объекта по id
    Task<byte[]> GetImageBytesAsync(Guid pizzaId);
    Task<Pizza?> CreateAsync(PizzaCreationModel item); // создание объекта
    Task<Pizza?> UpdateAsync(Pizza item); // обновление объекта
    Task<Pizza?> DeleteAsync(Guid id); // удаление объекта по id





}
