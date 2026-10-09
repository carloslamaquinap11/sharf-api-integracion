namespace Persistence;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notification", schema: "SERVICE");
        builder.Property(p => p.Id).IsRequired();
        builder.HasKey(p => p.Id);
    }
}