namespace Persistence
{
    using Domain;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    public abstract class EntityMapBase<T> : IEntityTypeConfiguration<T> where T : BaseDomainModel
    {
        void IEntityTypeConfiguration<T>.Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(b => b.CreatedBy).HasMaxLength(100).IsRequired();
            builder.Property(b => b.UpdatedBy).HasMaxLength(100).IsRequired(false);
            builder.Property(b => b.CreatedDate).IsRequired();
            builder.Property(b => b.UpdatedDate).IsRequired(false);
            builder.Property(b => b.IsActive).HasDefaultValue(true);
            Configure(builder);
        }

        protected abstract void Configure(EntityTypeBuilder<T> builder);
    }
}