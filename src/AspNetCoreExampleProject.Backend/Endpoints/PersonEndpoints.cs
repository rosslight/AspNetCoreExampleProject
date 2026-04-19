using AspNetCoreExampleProject.Api.Requests.Person;
using AspNetCoreExampleProject.Api.Responses.Person;
using AspNetCoreExampleProject.Backend.Database;
using AspNetCoreExampleProject.Backend.Database.Models;
using AspNetCoreExampleProject.Backend.Endpoints.ResponseExtensions.Person;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreExampleProject.Backend.Endpoints;

public static class PersonEndpoints
{
    public static void MapPersonEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet(
            "/persons",
            async Task<Ok<PersonListResponse>> (ApplicationDbContext context, CancellationToken ct) =>
            {
                var persons = await context
                    .Persons.OrderBy(person => person.LastName)
                    .ThenBy(person => person.FirstName)
                    .Select(person => person.ToPersonMinimalResponse())
                    .ToListAsync(ct);

                return TypedResults.Ok(new PersonListResponse { Persons = persons });
            }
        );

        group.MapPost(
            "/persons",
            async Task<Results<Ok<PersonDetailResponse>, BadRequest>> (
                CreatePersonRequest request,
                ApplicationDbContext context,
                CancellationToken ct
            ) =>
            {
                var city = await context.Cities.FindAsync(request.CityId, ct);

                if (city == null)
                    return TypedResults.BadRequest();

                var entity = await context.Persons.AddAsync(
                    new Person
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        City = city,
                    },
                    ct
                );
                await context.SaveChangesAsync(ct);

                return TypedResults.Ok(entity.Entity.ToPersonDetailResponse());
            }
        );
    }
}
