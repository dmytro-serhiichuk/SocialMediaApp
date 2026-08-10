using WebApp1.Api.Data;
using WebApp1.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace WebApp1.Api.Services;

public class UserService(AppDbContext dbContext, IPasswordHasher<User> passwordHasher)
{
    public async Task<User?> Create(CreateUserRequest createUserRequest)
    {
        try
        {
            var exists = await dbContext.Users
                        .AnyAsync(u => u.Email == createUserRequest.Email);

            if (exists)
                return null;

            var user = new User
            {
                UserName = createUserRequest.UserName,
                Email = createUserRequest.Email
            };
            user.PasswordHash = passwordHasher.HashPassword(user, createUserRequest.Password);

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return user;
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            return null;
        }
    }
}