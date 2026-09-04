namespace AspNetCoreExampleProject.Api.Responses.Health;

public sealed class HealthCheckEntryResponse
{
    public required string Status { get; set; }

    public string? Description { get; set; }
}
