using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Interfaces;

public interface IPromocodeRepository
{
    Task<ICollection<Promocode>> GetAllAsync(); // получение всех объектов
    Task<Promocode?> GetAsync(Guid id); // получение одного объекта по id
    Task<Promocode?> CreateAsync(PromocodeCreationModel item); // создание объекта
    Task<Promocode?> UpdateAsync(Promocode item); // обновление объекта
    Task<Promocode?> DeleteAsync(Guid id); // удаление объекта по id
    Task SaveChangesAsync();
}
