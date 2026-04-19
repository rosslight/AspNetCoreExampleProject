namespace AspNetCoreExampleProject.Api.Responses.Person;

public sealed class PersonListResponse
{
    public List<PersonMinimalResponse> Persons { get; set; } = [];
}
