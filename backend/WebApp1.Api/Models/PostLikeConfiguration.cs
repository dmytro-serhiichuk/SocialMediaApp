using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp1.Api.Models;

public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.ToTable("postLikes");

        builder.HasKey(x => new { x.PostId, x.UserId });

        builder.HasOne(x => x.User)
            .WithMany(u => u.LikedPosts)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(x => x.PostId);
    }
}