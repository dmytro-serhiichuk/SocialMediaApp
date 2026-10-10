using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMediaApp.Api.Models;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Comments)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Post)
            .WithMany(x => x.Comments)
            .HasForeignKey(p => p.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ReplyToUser)
            .WithMany()
            .HasForeignKey(x => x.ReplyToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.ParentComment)
            .WithMany(x => x.Replies)
            .HasForeignKey(x => x.ParentCommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.CreatedAt, x.Id, x.PostId });
        builder.HasIndex(x => new { x.CreatedAt, x.Id, x.ParentCommentId });
    }
}