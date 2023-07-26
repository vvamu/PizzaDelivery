
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Domain.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using PizzaDelivery.Application.Helpers;
using PizzaDelivery.Application.Services.Interfaces;
using AutoMapper;
using PizzaDelivery.Domain.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PizzaDelivery.Application.Services.Implementation;

public class PizzaService : AbstractTransactionService, IPizzaService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PizzaService(ApplicationDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<Pizza?> GetAsync(Guid id)
    {
        return await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<ICollection<Pizza>> GetAllAsync()
    {

        return await _context.Pizzas.ToListAsync();
    }

    public PagedList<Pizza> GetAllAsync(QueryStringParameters ownerParameters)
    {
        var pizzas = _context.Pizzas.OrderBy(on => on.Name);

        return PagedList<Pizza>.ToPagedList(pizzas,
        ownerParameters.PageNumber,
        ownerParameters.PageSize);
    }

    public async Task<Pizza> CreateAsync(PizzaCreateModel item)
    {
        var validator = new PizzaValidator();
        var validationResult = await validator.ValidateAsync(item);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;
            var errorsString = String.Concat(errors);
            throw new Exception(errorsString);
        }

        if (_context.Pizzas.Any(x => x.Name == item.Name)) throw new Exception("Pizza with such name alredy exists");
        //if (item.ImageFile == null || item.ImageFile.Length <= 0) throw new Exception("Error with file");

        var pizza = _mapper.Map<Pizza>(item);
        await _context.Pizzas.AddAsync(pizza);
        await UpsertImage(pizza, item.ImageFile);
        var db_item = await _context.Pizzas.FindAsync(pizza.Id);
        await _context.SaveChangesAsync();

        return db_item == null ? throw new NotImplementedException() : db_item;

    }
    public async Task<Pizza> UpdateAsync(Pizza item)
    {
        var validator = new PizzaValidator();
        var validationResult = await validator.ValidateAsync(_mapper.Map<PizzaCreateModel>(item));

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors;
            var errorsString = String.Concat(errors);
            throw new Exception(errorsString);
        }
        var db_item = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == item.Id);
        if (db_item == null) throw new KeyNotFoundException();
        var promo = _context.Pizzas.Update(item);
        await UpsertImage(promo.Entity, item.ImageFile);
        await _context.SaveChangesAsync();
        return promo.Entity;
    }
    public async Task<Pizza> DeleteAsync(Guid id)
    {
        var db_item = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == id);
        if (db_item == null) throw new KeyNotFoundException();
        var executionStrategy = CreateExecutionStrategy();
        EntityEntry<Pizza>? remotedItem = null;
        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await BeginTransactionAsync();
            try
            {
                File.Delete(db_item.ImagePath);
            }
            catch (Exception ex)
            {
                await RollbackAsync(transaction);
                throw new Exception("Failed to delete image", ex);
            }
            remotedItem = _context.Pizzas.Remove(db_item);
            await _context.SaveChangesAsync();
            await CommitAsync(transaction);
        });
        return remotedItem.Entity;
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private async Task<string> UpsertImage(Pizza pizza, IFormFile image)
    {
        if (image != null && image.Length > 0)
        {

            var fileExtension = Path.GetExtension(image.FileName);
            if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string parentDirectory = Directory.GetParent(currentDirectory).FullName;

                var localPath = "\\PizzaDelivery.Domain\\Images\\";
                string fileName = pizza.Name + fileExtension;
                string filePath = parentDirectory + localPath + fileName;

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
                pizza.ImagePath = filePath;
                pizza.ImageMime = fileExtension.Replace(".", "");
                return filePath;
            }
            else throw new BadImageFormatException();
        }
        else return null;//throw new FileNotFoundException();
    }
    public async Task<byte[]> GetImageBytesAsync(Guid pizzaId)
    {
        var pizza = await GetAsync(pizzaId);
        if (!_context.Pizzas.Any(x => x.Id == pizza.Id)) throw new Exception("No such Pizza");
        if (File.Exists(pizza.ImagePath))
        {
            byte[] image = await File.ReadAllBytesAsync(pizza.ImagePath);
            return image;
            //return File(image, pizza.ImageMime,);
        }
        else throw new FileNotFoundException();
    }

}
