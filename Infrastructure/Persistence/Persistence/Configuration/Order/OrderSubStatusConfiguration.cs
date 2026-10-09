namespace Persistence;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class OrderSubStatusConfiguration : IEntityTypeConfiguration<OrderSubStatus>
{
    public void Configure(EntityTypeBuilder<OrderSubStatus> builder)
    {
        builder.ToTable("OrderSubStatus", schema: "ORDER");
        builder.Property(p => p.Id).IsRequired();
        builder.HasKey(p => p.Id);

        builder.HasMany(b => b.Orders)
                .WithOne(b => b.OrderSubStatus)
                .HasForeignKey(b => b.IdOrderSubStatus).OnDelete(DeleteBehavior.Restrict);
    }
}