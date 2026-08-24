using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp1.Api.Models;

public class PostSaveConfiguration : IEntityTypeConfiguration<PostSave>
{
    public void Configure(EntityTypeBuilder<PostSave> builder)
    {
        builder.ToTable("postSaves");

        builder.HasKey(x => new { x.PostId, x.UserId });

        builder.HasOne(x => x.User)
            .WithMany(u => u.SavedPosts)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Post)
            .WithMany(p => p.Saves)
            .HasForeignKey(x => x.PostId);

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}