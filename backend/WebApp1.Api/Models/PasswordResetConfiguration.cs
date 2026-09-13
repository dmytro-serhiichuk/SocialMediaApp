using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp1.Api.Models;

public class PasswordResetConfiguration : IEntityTypeConfiguration<PasswordReset>
{
    public void Configure(EntityTypeBuilder<PasswordReset> builder)
    {
        builder.ToTable("passwordResets");
        
        builder.HasKey(x => x.UserId);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.HasIndex(x => x.ExpiresAt);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<PasswordReset>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}