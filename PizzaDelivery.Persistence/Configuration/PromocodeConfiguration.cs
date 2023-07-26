using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Persistence.Configuration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
        .HasOne(o => o.Promocode)
        .WithMany()
        .HasForeignKey(o => o.PromocodeId)
        .OnDelete(DeleteBehavior.SetNull);
    }
}
