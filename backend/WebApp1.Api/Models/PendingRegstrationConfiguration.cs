using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp1.Api.Models;

public class PendingRegstrationConfiguration : IEntityTypeConfiguration<PendingRegistration>
{
    public void Configure(EntityTypeBuilder<PendingRegistration> builder)
    {
        builder.ToTable("pendingRegistrations");
        
        builder.HasKey(i => i.Id);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.UserTag)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.ExpiresAt)
            .IsRequired();
    }
}