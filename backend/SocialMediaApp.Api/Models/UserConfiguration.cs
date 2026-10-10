using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMediaApp.Api.Data.Constants;

namespace SocialMediaApp.Api.Models;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(u => u.Id);

        builder.Property(u => u.PasswordHash)
            .IsRequired(false);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(320);

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName(UserIndexes.Email);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.UserTag)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.UserTag)
            .IsUnique()
            .HasDatabaseName(UserIndexes.UserTag);

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
    }
}