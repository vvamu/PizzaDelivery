using PizzaDelivery.Application.Helpers;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Services.Interfaces;

public interface IPromocodeService
{
    public PagedList<Promocode> GetAllAsync(QueryStringParameters queryStringParameters);
    Task<ICollection<Promocode>> GetAllAsync(); // получение всех объектов

    Task<Promocode?> GetAsync(Guid id); // получение одного объекта по id
    Task<Promocode?> CreateAsync(PromocodeCreateModel item); // создание объекта
    Task<Promocode?> UpdateAsync(Promocode item); // обновление объекта
    Task<Promocode?> DeleteAsync(Guid id); // удаление объекта по id
    Task SaveChangesAsync();
}
