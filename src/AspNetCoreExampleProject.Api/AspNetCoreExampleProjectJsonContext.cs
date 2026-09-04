using System.Text.Json.Serialization;
using AspNetCoreExampleProject.Api.Requests.City;
using AspNetCoreExampleProject.Api.Requests.Person;
using AspNetCoreExampleProject.Api.Responses.City;
using AspNetCoreExampleProject.Api.Responses.Health;
using AspNetCoreExampleProject.Api.Responses.Person;

namespace AspNetCoreExampleProject.Api;

// Requests
[JsonSerializable(typeof(CreateCityRequest))]
[JsonSerializable(typeof(CreatePersonRequest))]
[JsonSerializable(typeof(AddFriendRequest))]
[JsonSerializable(typeof(RemoveFriendRequest))]
// Responses
[JsonSerializable(typeof(CityDetailResponse))]
[JsonSerializable(typeof(CityMinimalResponse))]
[JsonSerializable(typeof(PersonDetailResponse))]
[JsonSerializable(typeof(PersonMinimalResponse))]
[JsonSerializable(typeof(CityListResponse))]
[JsonSerializable(typeof(PersonListResponse))]
[JsonSerializable(typeof(HealthCheckResponse))]
[JsonSerializable(typeof(HealthCheckEntryResponse))]
// Setup
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
public sealed partial class AspNetCoreExampleProjectJsonContext : JsonSerializerContext;
