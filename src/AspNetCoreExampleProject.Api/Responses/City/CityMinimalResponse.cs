namespace AspNetCoreExampleProject.Api.Responses.City;

public class CityMinimalResponse
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int InhabitantCount { get; set; }
}
