using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMediaApp.Api.Models;

public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
{
    public void Configure(EntityTypeBuilder<UserFollow> builder)
    {
        builder.ToTable("userFollows");

        builder.HasKey(x => new { x.FollowerId, x.FollowingId });

        builder.HasOne(x => x.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(x => x.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Following)
            .WithMany(u => u.Followers)
            .HasForeignKey(x => x.FollowingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}