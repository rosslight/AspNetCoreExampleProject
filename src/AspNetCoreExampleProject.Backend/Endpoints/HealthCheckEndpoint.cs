using AspNetCoreExampleProject.Api.Responses.Health;
using AspNetCoreExampleProject.Backend.Endpoints.ResponseExtensions.Health;
using AspNetCoreExampleProject.Backend.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCoreExampleProject.Backend.Endpoints;

public static class HealthCheckEndpoint
{
    public static void MapCustomHealthCheck(this WebApplication app)
    {
        app.MapGet(
            "/health",
            async Task<Results<Ok<HealthCheckResponse>, InternalServerError<HealthCheckResponse>>> (
                HealthCheckService healthCheckService,
                IAppInformationService appInformationService,
                CancellationToken ct
            ) =>
            {
                var report = await healthCheckService.CheckHealthAsync(ct);

                var response = report.ToHealthCheckResponse(appInformationService.Version);

                return report.Status == HealthStatus.Unhealthy
                    ? TypedResults.InternalServerError(response)
                    : TypedResults.Ok(response);
            }
        );
    }
}
