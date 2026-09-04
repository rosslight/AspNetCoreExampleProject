namespace AspNetCoreExampleProject.Api.Responses.Health;

public sealed class HealthCheckResponse
{
    public required string Status { get; set; }

    public required string Version { get; set; }

    public required Dictionary<string, HealthCheckEntryResponse> Entries { get; set; }
}
