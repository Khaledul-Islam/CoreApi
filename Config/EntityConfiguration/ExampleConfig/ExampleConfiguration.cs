using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities.ExampleEntity;

namespace Config.EntityConfiguration.ExampleConfig
{
    public class ExampleConfiguration : IEntityTypeConfiguration<Example>
    {
        public void Configure(EntityTypeBuilder<Example> builder)
        {
            builder.ToTable(nameof(Example));
            builder.HasKey(e=>e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Extra1).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Extra2).IsRequired().HasMaxLength(100);
            builder.Property(e => e.CreatedDate).IsRequired().HasMaxLength(100);
            builder.Property(e => e.IsActive).HasDefaultValue(false);

        }
    }
}
