using AspNetCoreExampleProject.Api.Requests.City;
using AspNetCoreExampleProject.Api.Responses.City;
using AspNetCoreExampleProject.Backend.Database;
using AspNetCoreExampleProject.Backend.Database.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AspNetCoreExampleProject.Backend.Endpoints;

public static class CityEndpoints
{
    public static void MapCityEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet(
            "/cities",
            async Task<Ok<CityListResponse>> (ApplicationDbContext context, CancellationToken ct) =>
            {
                return TypedResults.Ok(
                    new CityListResponse
                    {
                        Cities = context
                            .Cities.Select(city => new CityMinimalResponse
                            {
                                Id = city.Id,
                                Name = city.Name,
                                InhabitantCount = city.Persons.Count,
                            })
                            .ToList(),
                    }
                );
            }
        );

        group.MapPost(
            "/cities",
            async Task<Results<Ok<CityDetailResponse>, NotFound>> (
                CreateCityRequest request,
                ApplicationDbContext context,
                CancellationToken ct
            ) =>
            {
                var entity = await context.Cities.AddAsync(new City { Name = request.Name }, ct);
                await context.SaveChangesAsync(ct);

                return TypedResults.Ok(new CityDetailResponse { Id = entity.Entity.Id, Name = entity.Entity.Name });
            }
        );
    }
}
