using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp1.Api.Models;

public class NewLoginSecurityRequestConfiguration : IEntityTypeConfiguration<NewLoginSecurityRequest>
{
    public void Configure(EntityTypeBuilder<NewLoginSecurityRequest> builder)
    {
        builder.ToTable("newLoginSecurityRequests");
        
        builder.HasKey(x => x.UserId);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.Property(x => x.ExpiresAt).IsRequired();

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<NewLoginSecurityRequest>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}