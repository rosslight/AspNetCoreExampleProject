using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCoreExampleProject.Backend.Database;

public class DbHealthCheck(ApplicationDbContext context) : IHealthCheck
{
    private readonly ApplicationDbContext _context = context;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);

            return HealthCheckResult.Healthy("Database is up");
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy("Database not up");
        }
    }
}
