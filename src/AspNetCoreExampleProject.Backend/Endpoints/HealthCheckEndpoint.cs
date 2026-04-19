using System.Text.Json;
using AspNetCoreExampleProject.Api;
using AspNetCoreExampleProject.Api.Responses.Health;
using AspNetCoreExampleProject.Backend.Helpers;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace AspNetCoreExampleProject.Backend.Endpoints;

public static class HealthCheckEndpoint
{
    public static void MapCustomHealthCheck(this WebApplication app)
    {
        app.MapHealthChecks(
            "/health",
            new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var appInformationService = app.Services.GetRequiredService<IAppInformationService>();

                    context.Response.ContentType = "application/json";

                    var response = new HealthCheckResponse
                    {
                        Status = report.Status.ToString(),
                        Version = appInformationService.Version,
                    };

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(
                            response,
                            AspNetCoreExampleProjectJsonContext.Default.HealthCheckResponse
                        )
                    );
                },
            }
        );
    }
}
