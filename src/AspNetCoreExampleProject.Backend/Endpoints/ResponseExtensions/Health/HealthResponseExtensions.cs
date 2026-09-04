using AspNetCoreExampleProject.Api.Responses.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCoreExampleProject.Backend.Endpoints.ResponseExtensions.Health;

public static class HealthResponseExtensions
{
    extension(HealthReport report)
    {
        public HealthCheckResponse ToHealthCheckResponse(string version)
        {
            return new HealthCheckResponse
            {
                Status = report.Status.ToString(),
                Version = version,
                Entries = report.Entries.ToDictionary(
                    entry => entry.Key,
                    entry => new HealthCheckEntryResponse
                    {
                        Status = entry.Value.Status.ToString(),
                        Description = entry.Value.Description,
                    }
                ),
            };
        }
    }
}
