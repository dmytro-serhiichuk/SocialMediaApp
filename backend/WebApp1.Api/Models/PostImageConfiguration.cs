using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp1.Api.Models;

public class PostImageConfiguration : IEntityTypeConfiguration<PostImage>
{
    public void Configure(EntityTypeBuilder<PostImage> builder)
    {
        builder.ToTable("postImages");
        
        builder.HasKey(p => p.Id);

        builder.Property(p => p.StorageKey)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(p => p.Width)
            .IsRequired();

        builder.Property(p => p.Height)
            .IsRequired();

        builder.Property(p => p.Order)
            .IsRequired();

        builder.HasIndex(p => new { p.PostId, p.Order })
            .IsUnique();
        
        builder.HasOne(x => x.Post)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}