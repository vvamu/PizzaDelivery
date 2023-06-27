namespace PizzaDelivery.Services;

public interface IPizzaRepository :IRepository<Pizza>
{ 
    Task<ICollection<Pizza>> GetAllAsync(); // получение всех объектов
    Task<Pizza> GetAsync(Guid id); // получение одного объекта по id
    Task<Pizza> CreateAsync(Pizza item); // создание объекта
    Task<Pizza> UpdateAsync(Pizza item); // обновление объекта
    Task<Pizza> DeleteAsync(Guid id); // удаление объекта по id
}
