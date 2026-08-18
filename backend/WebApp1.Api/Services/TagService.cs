using Microsoft.EntityFrameworkCore;
using Npgsql;
using WebApp1.Api.Data;
using WebApp1.Api.DTOs;
using WebApp1.Api.Models;

namespace WebApp1.Api.Services;

public class TagService(AppDbContext dbContext)
{
    public async Task<List<Tag>> CreateOrGetTagsByName(IEnumerable<string> names)
    {
        var existing = await dbContext.Tags
            .Where(t => names.Contains(t.Name))
            .ToListAsync();

        var existingNames = existing.Select(t => t.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missing = names.Where(n => !existingNames.Contains(n)).ToList();
        if (missing.Count == 0)
            return existing;

        var newTags = missing.Select(n => new Tag { Name = n }).ToList();
        
        dbContext.Tags.AddRange(newTags);

        try
        {
            await dbContext.SaveChangesAsync();
            return existing.Concat(newTags).ToList();
        }
        catch (DbUpdateException ex) 
        when (ex.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            foreach (var t in newTags)
                dbContext.Entry(t).State = EntityState.Detached;
                
            var reloaded = await dbContext.Tags
                .Where(t => missing.Contains(t.Name))
                .ToListAsync();

            return existing.Concat(reloaded).ToList();
        }
    }

    public async Task<List<TagSearchResponse>> FindTagsByName(TagSearchRequest searchRequest, int limit = 5)
    {
        return await dbContext.Tags
            .Where(t => EF.Functions.Like(t.Name, $"{searchRequest.Query}%"))
            .OrderBy(t => t.Name)
            .Take(limit)
            .Select(t => new TagSearchResponse
            {
                Name = t.Name,
                UseCount = t.Posts.Count
            })
            .ToListAsync();
    }
}