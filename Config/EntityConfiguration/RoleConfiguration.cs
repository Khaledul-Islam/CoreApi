using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities.Identity;

namespace Config.EntityConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(nameof(Role));
            builder.HasKey(a => a.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(15);

            builder.Property(p => p.Description).IsRequired().HasMaxLength(100);
        }
    }
}