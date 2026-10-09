namespace Persistence;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class TrackingConfiguration : IEntityTypeConfiguration<Tracking>
{
    public void Configure(EntityTypeBuilder<Tracking> builder)
    {
        builder.ToTable("Tracking", schema: "ORDER");
        builder.Property(p => p.Id).IsRequired();
        builder.HasKey(p => p.Id);

        builder.HasMany(b => b.Files)
                .WithOne(b => b.Tracking)
                .HasForeignKey(b => b.IdTracking).OnDelete(DeleteBehavior.Restrict);
    }
}