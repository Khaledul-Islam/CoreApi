using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Models.Entities.Files;

namespace Config.EntityConfiguration
{
    public class FileModelConfiguration : IEntityTypeConfiguration<FileModel>
    {
        public void Configure(EntityTypeBuilder<FileModel> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);

            builder.Property(p => p.FileType).IsRequired().HasMaxLength(30);

            builder.Property(p => p.Extension).IsRequired().HasMaxLength(5);
            builder.Property(p => p.FilePath).HasMaxLength(500);

            builder.Property(p => p.Description).HasMaxLength(20);
        }
    }
}
