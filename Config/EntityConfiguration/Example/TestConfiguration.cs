using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities.Example;

namespace Config.EntityConfiguration.Example
{
    public class TestConfiguration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            builder.ToTable(nameof(Test));
            builder.HasKey(e=>e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Extra1).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Extra2).IsRequired().HasMaxLength(100);
            builder.Property(e => e.CreatedDate).IsRequired().HasMaxLength(100);
            builder.Property(e => e.IsActive).HasDefaultValue(false);

        }
    }
}
