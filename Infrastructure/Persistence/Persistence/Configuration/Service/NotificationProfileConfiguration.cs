namespace Persistence;

using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class NotificationProfileConfiguration : IEntityTypeConfiguration<NotificationProfile>
{
    public void Configure(EntityTypeBuilder<NotificationProfile> builder)
    {
        builder.ToTable("NotificationProfile", schema: "SERVICE");
        builder.Property(p => p.Id).IsRequired();
        builder.HasKey(p => p.Id);

        builder.HasMany(b => b.Clients)
                .WithOne(b => b.NotificationProfile)
                .HasForeignKey(b => b.IdNotificationProfile).OnDelete(DeleteBehavior.Restrict);
    }
}