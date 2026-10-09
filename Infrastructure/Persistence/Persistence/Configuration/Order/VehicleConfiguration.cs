namespace Persistence;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicle", schema: "ORDER");
        builder.Property(p => p.Id).IsRequired();
        builder.HasKey(p => p.Id);

        builder.HasMany(b => b.Trackings)
                .WithOne(b => b.Vehicle)
                .HasForeignKey(b => b.IdVehicle).OnDelete(DeleteBehavior.Restrict);
    }
}