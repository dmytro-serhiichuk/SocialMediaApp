using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMediaApp.Api.Models;

public class PostImageConfiguration : IEntityTypeConfiguration<PostImage>
{
    public void Configure(EntityTypeBuilder<PostImage> builder)
    {
        builder.ToTable("postImages");
        
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Order)
            .IsRequired();

        builder.HasIndex(p => new { p.PostId, p.Order })
            .IsUnique();
        
        builder.HasOne(x => x.Post)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Image)
            .WithMany()
            .HasForeignKey(x => x.ImageId);
    }
}