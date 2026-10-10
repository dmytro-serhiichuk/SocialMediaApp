using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMediaApp.Api.Models;

public class NewLoginSecurityRequestConfiguration : IEntityTypeConfiguration<NewLoginSecurityRequest>
{
    public void Configure(EntityTypeBuilder<NewLoginSecurityRequest> builder)
    {
        builder.ToTable("newLoginSecurityRequests");
        
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.HasIndex(x => x.ExpiresAt);

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Session)
            .WithOne()
            .HasForeignKey<NewLoginSecurityRequest>(x => x.SessionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}