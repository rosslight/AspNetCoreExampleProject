namespace AspNetCoreExampleProject.Api.Requests.Person;

public class CreatePersonRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public int CityId { get; set; }
}
