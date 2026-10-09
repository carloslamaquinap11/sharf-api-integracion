namespace Persistence;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Order", schema: "ORDER");
        builder.Property(p => p.Id).IsRequired();
        builder.HasKey(p => p.Id);

        builder.HasMany(b => b.Notifications)
                .WithOne(b => b.Order)
                .HasForeignKey(b => b.IdOrder).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Trackings)
                .WithOne(b => b.Order)
                .HasForeignKey(b => b.IdOrder).OnDelete(DeleteBehavior.Restrict);
    }
}