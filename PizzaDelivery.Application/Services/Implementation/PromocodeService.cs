using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Application.Helpers;
using PizzaDelivery.Application.Services.Interfaces;
using PizzaDelivery.Domain.Models;
using PizzaDelivery.Domain.Validators;

namespace PizzaDelivery.Application.Services.Implementation;


public class PromocodeService : IPromocodeService
{
    private ApplicationDbContext _context;
    private IMapper _mapper;

    public PromocodeService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<Promocode> GetAsync(Guid id)
    {
        return await _context.Promocodes.FirstOrDefaultAsync(x => x.Id == id);
    }

    public PagedList<Promocode> GetAllAsync(QueryStringParameters queryStringParameters)
    {
        var items = _context.Promocodes;
        return PagedList<Promocode>.ToPagedList(items, queryStringParameters.PageNumber, queryStringParameters.PageSize);

    }

    public async Task<ICollection<Promocode>> GetAllAsync()
    {
        return await _context.Promocodes.ToListAsync();
    }


    public async Task<Promocode> CreateAsync(PromocodeCreateModel item)
    {
        var validator = new PromocodeValidator();
        var validationResult = await validator.ValidateAsync(item);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;
            var errorsString = String.Concat(errors);
            throw new Exception(errorsString);
        }

        var promocode = new Promocode()
        {
            Value = item.Value,
            ExpireDate = item.ExpireDate,
            SalePercent = item.SalePercent,
        } ?? _mapper.Map<Promocode>(item);

        await _context.Promocodes.AddAsync(promocode);
        await _context.SaveChangesAsync();
        var items = _context.Promocodes.ToList();
       
        var db_item = await _context.Promocodes.FindAsync(promocode.Id);
        return db_item == null ? throw new NotImplementedException() : db_item;
    }
    public async Task<Promocode> UpdateAsync(Promocode item)
    {
        var validator = new PromocodeValidator();
        var validationResult = await validator.ValidateAsync(_mapper.Map<PromocodeCreateModel>(item));

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;
            var errorsString = String.Concat(errors);
            throw new Exception(errorsString);
        }

        var db_item = await _context.Promocodes.FirstOrDefaultAsync(x => x.Id == item.Id);
        if (db_item == null) throw new KeyNotFoundException();
        var promo = _context.Promocodes.Update(item);
        await _context.SaveChangesAsync();
        return promo.Entity;
    }
    public async Task<Promocode> DeleteAsync(Guid id)
    {
        var db_item = await _context.Promocodes.FirstOrDefaultAsync(x => x.Id == id);
        if (db_item == null) throw new KeyNotFoundException();
        var remotedItem = _context.Promocodes.Remove(db_item);
        await _context.SaveChangesAsync();
        return remotedItem.Entity;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }


}
