
using Microsoft.EntityFrameworkCore;
using WebApp1.Api.Data;

namespace WebApp1.Api.Services;

public class ExpiredDataCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExpiredDataCleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(6);
    private const int BatchSize = 5000;

    public ExpiredDataCleanupService(IServiceScopeFactory scopeFactory, ILogger<ExpiredDataCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(_interval);

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var now = DateTimeOffset.UtcNow;

                await DeleteExpiredInBatchesAsync("EmailChangeRequests", async ct =>
                {
                    var ids = await db.EmailChangeRequests
                        .Where(x => x.ExpiresAt < now)
                        .OrderBy(x => x.UserId)
                        .Select(x => x.UserId)
                        .Take(BatchSize)
                        .ToListAsync(ct);

                    if (ids.Count == 0) return 0;

                    return await db.EmailChangeRequests
                        .Where(x => ids.Contains(x.UserId))
                        .ExecuteDeleteAsync(ct);
                }, cancellationToken);

                await DeleteExpiredInBatchesAsync("NewLoginSecurityRequests", async ct =>
                {
                    var ids = await db.NewLoginSecurityRequests
                        .Where(x => x.ExpiresAt < now)
                        .OrderBy(x => x.UserId)
                        .Select(x => x.UserId)
                        .Take(BatchSize)
                        .ToListAsync(ct);

                    if (ids.Count == 0) return 0;

                    return await db.NewLoginSecurityRequests
                        .Where(x => ids.Contains(x.UserId))
                        .ExecuteDeleteAsync(ct);
                }, cancellationToken);

                await DeleteExpiredInBatchesAsync("PasswordResets", async ct =>
                {
                    var ids = await db.PasswordResets
                        .Where(x => x.ExpiresAt < now)
                        .OrderBy(x => x.UserId)
                        .Select(x => x.UserId)
                        .Take(BatchSize)
                        .ToListAsync(ct);

                    if (ids.Count == 0) return 0;

                    return await db.PasswordResets
                        .Where(x => ids.Contains(x.UserId))
                        .ExecuteDeleteAsync(ct);
                }, cancellationToken);

                await DeleteExpiredInBatchesAsync("PendingRegistrations", async ct =>
                {
                    var ids = await db.PendingRegistrations
                        .Where(x => x.ExpiresAt < now)
                        .OrderBy(x => x.Id)
                        .Select(x => x.Id)
                        .Take(BatchSize)
                        .ToListAsync(ct);

                    if (ids.Count == 0) return 0;

                    return await db.PendingRegistrations
                        .Where(x => ids.Contains(x.Id))
                        .ExecuteDeleteAsync(ct);
                }, cancellationToken);

                await DeleteExpiredInBatchesAsync("Sessions", async ct =>
                {
                    var ids = await db.Sessions
                        .Where(x => x.RefreshTokenExpiresAt < now)
                        .OrderBy(x => x.Id)
                        .Select(x => x.Id)
                        .Take(BatchSize)
                        .ToListAsync(ct);

                    if (ids.Count == 0) return 0;

                    return await db.Sessions
                        .Where(x => ids.Contains(x.Id))
                        .ExecuteDeleteAsync(ct);
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while cleaning up expired data");
            }
        }
    }

    private async Task<int> DeleteExpiredInBatchesAsync(
        string table, 
        Func<CancellationToken, Task<int>> deleteBatchAsync, 
        CancellationToken cancellationToken)
    {
        var total = 0;
        int deletedInBatch;

        do
        {
            deletedInBatch = await deleteBatchAsync(cancellationToken);
            total += deletedInBatch;

            await Task.Delay(100, cancellationToken);
        } while (deletedInBatch == BatchSize && !cancellationToken.IsCancellationRequested);

        if (total > 0)
        {
            _logger.LogInformation("Deleted {Count} expired rows from {Table}", total, table);
        }

        return total;
    }
}