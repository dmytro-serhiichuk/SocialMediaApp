using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp1.Api.Models;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RefreshTokenHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(s => s.PreviousRefreshTokenHash)
            .HasMaxLength(64);

        builder.Property(s => s.LastIpAddress).HasMaxLength(45);
        builder.Property(s => s.LastUserAgent).HasMaxLength(256);

        builder.HasOne(x => x.User)
            .WithMany(t => t.Sessions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.RefreshTokenHash)
            .IsUnique();
        
        builder.HasIndex(s => s.PreviousRefreshTokenHash);
        builder.HasIndex(s => s.RefreshTokenExpiresAt);
    }
}