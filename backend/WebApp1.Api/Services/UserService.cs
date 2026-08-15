using WebApp1.Api.Data;
using WebApp1.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using WebApp1.Api.DTOs;

namespace WebApp1.Api.Services;

public class UserService(AppDbContext dbContext, IPasswordHasher<User> passwordHasher)
{
    public async Task<User?> GetById(int id)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task<User?> GetByUserTag(string tag)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.UserTag == tag);
    }

    public async Task<User?> Login(LoginRequest loginRequest)
    {
        var login = loginRequest.Login.Trim();

        User? user;

        // TODO: add actual email validation
        if (login.Contains('@'))
            user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == login);
        else
            user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserTag == login);

        if (user is null)
            return null;

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, loginRequest.Password
        );

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            return null;

        return user;
    }

    public async Task<User?> Create(RegisterRequest registerRequest)
    {
        try
        {
            var exists = await dbContext.Users
                        .AnyAsync(u => u.Email == registerRequest.Email);

            if (exists)
                return null;

            var user = new User
            {
                UserName = registerRequest.UserName,
                UserTag = registerRequest.UserTag,
                Email = registerRequest.Email
            };
            user.PasswordHash = passwordHasher.HashPassword(user, registerRequest.Password);

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