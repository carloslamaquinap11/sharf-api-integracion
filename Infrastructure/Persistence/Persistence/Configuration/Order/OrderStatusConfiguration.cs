namespace Persistence;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.ToTable("OrderStatus", schema: "ORDER");
        builder.Property(p => p.Id).IsRequired();
        builder.HasKey(p => p.Id);

        builder.HasMany(b => b.Files)
                .WithOne(b => b.OrderStatus)
                .HasForeignKey(b => b.IdOrderStatus).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Orders)
                .WithOne(b => b.OrderStatus)
                .HasForeignKey(b => b.IdOrderStatus).OnDelete(DeleteBehavior.Restrict);
    }
}