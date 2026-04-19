using AspNetCoreExampleProject.Api.Responses.City;

namespace AspNetCoreExampleProject.Api.Responses.Person;

public sealed record PersonDetailResponse : PersonMinimalResponse
{
    public required CityMinimalResponse City { get; set; }

    public required ICollection<PersonMinimalResponse> Friends { get; set; }
}
