using WebApp1.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApp1.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();
    public DbSet<UserFollow> UserFollows => Set<UserFollow>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<PostTag> PostTags => Set<PostTag>();
    public DbSet<PostSave> PostSaves => Set<PostSave>();
    public DbSet<Image> Images => Set<Image>();
    public DbSet<PostImage> PostImages => Set<PostImage>();
    public DbSet<EmailChangeRequest> EmailChangeRequests => Set<EmailChangeRequest>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<PendingRegistration> PendingRegistrations => Set<PendingRegistration>();
    public DbSet<PasswordReset> PasswordResets => Set<PasswordReset>();
    public DbSet<NewLoginSecurityRequest> NewLoginSecurityRequests => Set<NewLoginSecurityRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}