using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMediaApp.Api.Models;

public class CommentLikeConfiguration : IEntityTypeConfiguration<CommentLike>
{
    public void Configure(EntityTypeBuilder<CommentLike> builder)
    {
        builder.ToTable("commentLikes");

        builder.HasKey(x => new { x.CommentId, x.UserId });

        builder.HasOne(x => x.User)
            .WithMany(u => u.LikedComments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Comment)
            .WithMany(p => p.Likes)
            .HasForeignKey(x => x.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}