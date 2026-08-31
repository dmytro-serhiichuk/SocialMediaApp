using Microsoft.EntityFrameworkCore;
using WebApp1.Api.Data;
using WebApp1.Api.DTOs;
using WebApp1.Api.Exceptions;
using WebApp1.Api.Extensions;

namespace WebApp1.Api.Services;

public class SettingsService(AppDbContext dbContext)
{
    public async Task<SettingsDataResponse?> GetData(int userId)
    {
        return await dbContext.Users
            .Where(x => x.Id == userId)
            .Select(x => new SettingsDataResponse
            {
                Id = x.Id,
                UserName = x.UserName,
                UserTag = x.UserTag,
                Email = x.Email
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdateProfile(int userId, SettingsProfileRequest profileRequest)
    {
        var user = await dbContext.Users.SingleAsync(x => x.Id == userId);

        user.UserName = profileRequest.UserName;
        user.UserTag = profileRequest.UserTag;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new UserTagAlreadyTakenException();
        }
    }
}