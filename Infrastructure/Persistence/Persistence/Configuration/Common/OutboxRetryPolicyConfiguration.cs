namespace Persistence;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain;

internal sealed class OutboxRetryPolicyConfiguration : IEntityTypeConfiguration<OutboxRetryPolicy>
{
    public void Configure(EntityTypeBuilder<OutboxRetryPolicy> builder)
    {
        builder.ToTable("OutboxRetryPolicy", schema: "ORDER");
        builder.Property(p => p.Id).IsRequired();
        builder.HasKey(p => p.Id);
    }
}