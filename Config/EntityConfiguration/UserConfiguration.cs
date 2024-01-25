using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities.Identity;

namespace Config.EntityConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.HasKey(a => a.Id);

            builder.Property(p => p.UserName).IsRequired().HasMaxLength(40).HasColumnType("varchar");

            builder.Property(p => p.Firstname).IsRequired().HasMaxLength(35);

            builder.Property(p => p.Lastname).IsRequired().HasMaxLength(35);

            builder.Property(p => p.Email).IsRequired().HasMaxLength(320);

            builder.Property(p => p.IsActive).HasDefaultValue(true);

            builder.Property(p => p.Birthdate).HasColumnType("date");

            builder.Property(p => p.RefreshToken).HasMaxLength(50);

            builder.Property(p => p.RefreshTokenExpirationTime);
        }
    }
}
