using AspNetCoreExampleProject.Backend.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AspNetCoreExampleProject.Backend.Database;

public sealed class TimestampsInterceptor(TimeProvider time) : SaveChangesInterceptor
{
    private readonly TimeProvider _time = time;

    private DateTimeOffset UtcNow => _time.GetUtcNow();

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default
    )
    {
        UpdateTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void UpdateTimestamps(DbContext? context)
    {
        if (context is null)
            return;
        DateTimeOffset now = UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<Entity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(e => e.CreatedAt).CurrentValue = now;
                    entry.Property(e => e.ModifiedAt).CurrentValue = now;
                    break;
                case EntityState.Modified:
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.ModifiedAt).CurrentValue = now;
                    break;
            }
        }
    }
}
