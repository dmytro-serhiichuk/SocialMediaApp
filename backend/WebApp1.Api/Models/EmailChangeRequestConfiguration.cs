using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApp1.Api.Models;

public class EmailChangeRequestConfiguration : IEntityTypeConfiguration<EmailChangeRequest>
{
    public void Configure(EntityTypeBuilder<EmailChangeRequest> builder)
    {
        builder.ToTable("emailChangeRequests");
        
        builder.HasKey(x => x.UserId);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.Property(x => x.NewEmail).IsRequired();

        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.HasIndex(x => x.ExpiresAt);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<EmailChangeRequest>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}