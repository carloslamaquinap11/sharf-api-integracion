namespace Persistence;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class AuditoryConfiguration : IEntityTypeConfiguration<Auditory>
{
    public void Configure(EntityTypeBuilder<Auditory> builder)
    {
        builder.ToTable("Auditory", schema: "SERVICE");
        builder.Property(p => p.Id).IsRequired();
        builder.HasKey(p => p.Id);
    }
}