namespace Persistence;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.ToTable("File", schema: "ORDER");
        builder.Property(p => p.Id).IsRequired();
        builder.HasKey(p => p.Id);
    }
}