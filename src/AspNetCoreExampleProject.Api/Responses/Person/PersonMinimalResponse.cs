namespace AspNetCoreExampleProject.Api.Responses.Person;

public record PersonMinimalResponse
{
    public int Id { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
