namespace Persistence;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class DispatchTypeConfiguration : IEntityTypeConfiguration<DispatchType>
{
    public void Configure(EntityTypeBuilder<DispatchType> builder)
    {
        builder.ToTable("DispatchType", schema: "ORDER");
        builder.Property(p => p.Id).IsRequired();
        builder.HasKey(p => p.Id);

        builder.HasMany(b => b.Orders)
                .WithOne(b => b.DispatchType)
                .HasForeignKey(b => b.IdDispatchType).OnDelete(DeleteBehavior.Restrict);
    }
}