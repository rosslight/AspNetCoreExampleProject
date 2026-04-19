using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AspNetCoreExampleProject.Api;
using AspNetCoreExampleProject.Api.Requests.City;
using AspNetCoreExampleProject.Api.Requests.Person;
using AspNetCoreExampleProject.Api.Responses.City;
using AspNetCoreExampleProject.Api.Responses.Health;
using AspNetCoreExampleProject.Api.Responses.Person;

namespace AspNetCoreExampleProject.Client;

public sealed class BackendApiService(HttpClient httpClient)
{
    private readonly Lock _healthSnapshotLock = new();
    private BackendHealthSnapshot _healthSnapshot = new(
        false,
        null,
        null,
        "Waiting for first health check",
        DateTimeOffset.Now
    );

    public Uri? BaseAddress => httpClient.BaseAddress;

    public BackendHealthSnapshot HealthSnapshot
    {
        get
        {
            lock (_healthSnapshotLock)
            {
                return _healthSnapshot;
            }
        }
    }

    public async Task StartHealthPollingAsync(CancellationToken cancellationToken)
    {
        await RefreshHealthAsync();

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            await RefreshHealthAsync();
        }
    }

    public Task RefreshHealthAsync() => UpdateHealthAsync();

    public async Task<ApiResult<IReadOnlyList<CityMinimalResponse>>> GetCitiesAsync()
    {
        var response = await httpClient.GetAsync("/v1/cities");
        if (!response.IsSuccessStatusCode)
        {
            return await ApiResult<IReadOnlyList<CityMinimalResponse>>.FromErrorAsync(response);
        }

        var payload = await response.Content.ReadFromJsonAsync(
            AspNetCoreExampleProjectJsonContext.Default.CityListResponse
        );

        return ApiResult<IReadOnlyList<CityMinimalResponse>>.Success(payload?.Cities ?? []);
    }

    public async Task<ApiResult<CityDetailResponse>> CreateCityAsync(string name)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/v1/cities",
            new CreateCityRequest { Name = name },
            AspNetCoreExampleProjectJsonContext.Default.CreateCityRequest
        );

        if (!response.IsSuccessStatusCode)
        {
            return await ApiResult<CityDetailResponse>.FromErrorAsync(response);
        }

        var payload = await response.Content.ReadFromJsonAsync(
            AspNetCoreExampleProjectJsonContext.Default.CityDetailResponse
        );

        return payload == null
            ? ApiResult<CityDetailResponse>.Failure((int)HttpStatusCode.OK, "Response could not be read.", null)
            : ApiResult<CityDetailResponse>.Success(payload);
    }

    public async Task<ApiResult<IReadOnlyList<PersonMinimalResponse>>> GetPersonsAsync()
    {
        var response = await httpClient.GetAsync("/v1/persons");
        if (!response.IsSuccessStatusCode)
        {
            return await ApiResult<IReadOnlyList<PersonMinimalResponse>>.FromErrorAsync(response);
        }

        var payload = await response.Content.ReadFromJsonAsync(
            AspNetCoreExampleProjectJsonContext.Default.PersonListResponse
        );

        return ApiResult<IReadOnlyList<PersonMinimalResponse>>.Success(payload?.Persons ?? []);
    }

    public async Task<ApiResult<HealthCheckResponse>> GetHealthAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("/health");
            if (!response.IsSuccessStatusCode)
            {
                return await ApiResult<HealthCheckResponse>.FromErrorAsync(response);
            }

            var payload = await response.Content.ReadFromJsonAsync(
                AspNetCoreExampleProjectJsonContext.Default.HealthCheckResponse
            );

            return payload == null
                ? ApiResult<HealthCheckResponse>.Failure((int)HttpStatusCode.OK, "Response could not be read.", null)
                : ApiResult<HealthCheckResponse>.Success(payload);
        }
        catch (HttpRequestException ex)
        {
            return ApiResult<HealthCheckResponse>.Failure(null, ex.Message, null);
        }
        catch (TaskCanceledException ex)
        {
            return ApiResult<HealthCheckResponse>.Failure(null, ex.Message, null);
        }
        catch (JsonException ex)
        {
            return ApiResult<HealthCheckResponse>.Failure(null, ex.Message, null);
        }
    }

    public async Task<ApiResult<PersonDetailResponse>> CreatePersonAsync(string firstName, string lastName, int cityId)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/v1/persons",
            new CreatePersonRequest
            {
                FirstName = firstName,
                LastName = lastName,
                CityId = cityId,
            },
            AspNetCoreExampleProjectJsonContext.Default.CreatePersonRequest
        );

        if (!response.IsSuccessStatusCode)
        {
            return await ApiResult<PersonDetailResponse>.FromErrorAsync(response);
        }

        var payload = await response.Content.ReadFromJsonAsync(
            AspNetCoreExampleProjectJsonContext.Default.PersonDetailResponse
        );

        return payload == null
            ? ApiResult<PersonDetailResponse>.Failure((int)HttpStatusCode.OK, "Response could not be read.", null)
            : ApiResult<PersonDetailResponse>.Success(payload);
    }

    private async Task UpdateHealthAsync()
    {
        var result = await GetHealthAsync();

        if (result.IsSuccess && result.Value != null)
        {
            SetHealthSnapshot(
                new BackendHealthSnapshot(true, result.Value.Status, result.Value.Version, null, DateTimeOffset.Now)
            );
            return;
        }

        SetHealthSnapshot(
            new BackendHealthSnapshot(
                false,
                null,
                null,
                result.ErrorMessage ?? "Backend unavailable",
                DateTimeOffset.Now
            )
        );
    }

    private void SetHealthSnapshot(BackendHealthSnapshot snapshot)
    {
        lock (_healthSnapshotLock)
        {
            _healthSnapshot = snapshot;
        }
    }
}

public sealed record ApiResult<T>(bool IsSuccess, T? Value, int? StatusCode, string? ErrorMessage, string? ErrorBody)
{
    public static ApiResult<T> Success(T value) => new(true, value, null, null, null);

    public static ApiResult<T> Failure(int? statusCode, string? errorMessage, string? errorBody) =>
        new(false, default, statusCode, errorMessage, errorBody);

    public static async Task<ApiResult<T>> FromErrorAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        return Failure((int)response.StatusCode, response.ReasonPhrase, body);
    }
}

public sealed record BackendHealthSnapshot(
    bool IsReachable,
    string? Status,
    string? Version,
    string? ErrorMessage,
    DateTimeOffset LastUpdatedAt
);
