namespace PizzaDelivery.DomainRealize.Interfaces;

public interface IRepository<T>
{
    Task<ICollection<T>> GetAllAsync(); // получение всех объектов
    Task<T?> GetAsync(Guid id); // получение одного объекта по id
    Task<T?> CreateAsync(T item); // создание объекта
    Task<T?> UpdateAsync(T item); // обновление объекта
    Task<T?> DeleteAsync(Guid id); // удаление объекта по id
    Task SaveChangesAsync();
}
