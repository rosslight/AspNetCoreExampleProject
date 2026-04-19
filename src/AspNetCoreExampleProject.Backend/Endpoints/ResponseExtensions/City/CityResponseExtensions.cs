using AspNetCoreExampleProject.Api.Responses.City;
using AspNetCoreExampleProject.Backend.Endpoints.ResponseExtensions.Person;

namespace AspNetCoreExampleProject.Backend.Endpoints.ResponseExtensions.City;

public static class CityResponseExtensions
{
    extension(Database.Models.City city)
    {
        public CityDetailResponse ToCityDetailResponse()
        {
            return new CityDetailResponse
            {
                Id = city.Id,
                Name = city.Name,
                Persons = city.Persons.Select(p => p.ToPersonMinimalResponse()).ToList(),
            };
        }

        public CityMinimalResponse ToCityMinimalResponse()
        {
            return new CityMinimalResponse { Id = city.Id, Name = city.Name };
        }
    }
}
