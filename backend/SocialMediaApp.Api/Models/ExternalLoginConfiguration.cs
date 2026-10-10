using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMediaApp.Api.Models;

public class ExternalLoginConfiguration : IEntityTypeConfiguration<ExternalLogin>
{
    public void Configure(EntityTypeBuilder<ExternalLogin> builder)
    {
        builder.ToTable("externalLogins");
        
         builder.HasKey(x => x.Id);

        builder.Property(x => x.ProviderUserId)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(x => new
        {
            x.Provider,
            x.ProviderUserId
        })
        .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.ExternalLogins)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}