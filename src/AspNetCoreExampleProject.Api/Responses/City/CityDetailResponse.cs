using AspNetCoreExampleProject.Api.Responses.Person;

namespace AspNetCoreExampleProject.Api.Responses.City;

public class CityDetailResponse : CityMinimalResponse
{
    public ICollection<PersonMinimalResponse> Persons { get; set; } = [];
}
