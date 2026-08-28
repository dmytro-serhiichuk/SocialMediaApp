using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp1.Api.Models;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.ToTable("images");
        
         builder.HasKey(i => i.Id);

        builder.Property(i => i.StorageKey)
            .IsRequired()
            .HasMaxLength(512);

        builder.HasIndex(i => i.StorageKey)
            .IsUnique();

        builder.Property(i => i.Width)
            .IsRequired();

        builder.Property(i => i.Height)
            .IsRequired();
    }
}