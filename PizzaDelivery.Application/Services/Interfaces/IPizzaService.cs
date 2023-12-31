﻿using PizzaDelivery.Application.Helpers;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Services.Interfaces;

public interface IPizzaService
{
    public PagedList<Pizza> GetAllAsync(QueryStringParameters ownerParameters);
    public Task<ICollection<Pizza>> GetAllAsync();

    Task<Pizza?> GetAsync(Guid id); // получение одного объекта по id
    Task<byte[]> GetImageBytesAsync(Guid pizzaId);
    Task<Pizza?> CreateAsync(PizzaCreateModel item); // создание объекта
    Task<Pizza?> UpdateAsync(Pizza item); // обновление объекта
    Task<Pizza?> DeleteAsync(Guid id); // удаление объекта по id





}
