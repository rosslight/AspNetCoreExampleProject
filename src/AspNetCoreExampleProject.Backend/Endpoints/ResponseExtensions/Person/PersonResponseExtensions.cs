using AspNetCoreExampleProject.Api.Responses.Person;
using AspNetCoreExampleProject.Backend.Endpoints.ResponseExtensions.City;

namespace AspNetCoreExampleProject.Backend.Endpoints.ResponseExtensions.Person;

public static class PersonResponseExtensions
{
    extension(Database.Models.Person person)
    {
        public PersonMinimalResponse ToPersonMinimalResponse()
        {
            return new PersonMinimalResponse
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
            };
        }

        public PersonDetailResponse ToPersonDetailResponse()
        {
            return new PersonDetailResponse
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                City = person.City.ToCityMinimalResponse(),
                Friends = person
                    .FriendshipsInitiated.Select(friendship => new PersonMinimalResponse
                    {
                        Id = friendship.SecondPersonId,
                        FirstName = friendship.SecondPerson.FirstName,
                        LastName = friendship.SecondPerson.LastName,
                    })
                    .Concat(
                        person.FriendshipsReceived.Select(friendship => new PersonMinimalResponse
                        {
                            Id = friendship.FirstPersonId,
                            FirstName = friendship.FirstPerson.FirstName,
                            LastName = friendship.FirstPerson.LastName,
                        })
                    )
                    .ToList(),
            };
        }
    }
}
