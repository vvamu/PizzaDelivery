using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Interfaces;

public interface IPizzaRepository
{
    Task<ICollection<Pizza>> GetAllAsync(); // получение всех объектов
    Task<Pizza?> GetAsync(Guid id); // получение одного объекта по id
    Task<Pizza?> CreateAsync(PizzaCreationModel item); // создание объекта
    Task<Pizza?> UpdateAsync(Pizza item); // обновление объекта
    Task<Pizza?> DeleteAsync(Guid id); // удаление объекта по id
    Task SaveChangesAsync();
}
