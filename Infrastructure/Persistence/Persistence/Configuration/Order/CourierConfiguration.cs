namespace Persistence;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class CourierConfiguration : IEntityTypeConfiguration<Courier>
{
    public void Configure(EntityTypeBuilder<Courier> builder)
    {
        builder.ToTable("Courier", schema: "ORDER");
        builder.Property(p => p.Id).IsRequired();
        builder.HasKey(p => p.Id);

        builder.HasMany(b => b.Trackings)
                .WithOne(b => b.Courier)
                .HasForeignKey(b => b.IdCourier).OnDelete(DeleteBehavior.Restrict);
    }
}