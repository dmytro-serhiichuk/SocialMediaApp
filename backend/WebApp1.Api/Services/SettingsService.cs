using Microsoft.EntityFrameworkCore;
using WebApp1.Api.Data;
using WebApp1.Api.DTOs;

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
}