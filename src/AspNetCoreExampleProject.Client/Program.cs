using AspNetCoreExampleProject.Api.Responses.City;
using AspNetCoreExampleProject.Client;
using Spectre.Console;

var baseUrl = ResolveBaseUrl(args);
using var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
var backendApiService = new BackendApiService(httpClient);
using var pollingCts = new CancellationTokenSource();
var healthPollingTask = backendApiService.StartHealthPollingAsync(pollingCts.Token);
await backendApiService.RefreshHealthAsync();

try
{
    while (true)
    {
        RenderHeader(backendApiService);
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<MenuAction>()
                .Title("Choose an action")
                .UseConverter(action => action.Label)
                .AddChoices(MenuAction.All)
        );

        AnsiConsole.WriteLine();

        switch (choice.Key)
        {
            case "list-cities":
                await ListCitiesAsync(backendApiService);
                WaitForContinue();
                break;
            case "create-city":
                await CreateCityAsync(backendApiService);
                WaitForContinue();
                break;
            case "list-persons":
                await ListPersonsAsync(backendApiService);
                WaitForContinue();
                break;
            case "create-person":
                await CreatePersonAsync(backendApiService);
                WaitForContinue();
                break;
            case "exit":
                return;
        }
    }
}
finally
{
    await pollingCts.CancelAsync();

    try
    {
        await healthPollingTask;
    }
    catch (OperationCanceledException) { }
}

static string ResolveBaseUrl(string[] args)
{
    var cliValue = args.Select(arg => arg.Split('=', 2, StringSplitOptions.TrimEntries))
        .FirstOrDefault(parts => parts.Length == 2 && parts[0] == "--base-url");

    var configured = cliValue?[1];

    if (string.IsNullOrWhiteSpace(configured))
    {
        configured = Environment.GetEnvironmentVariable("BACKEND_URL");
    }

    if (string.IsNullOrWhiteSpace(configured))
    {
        configured = "http://localhost:5175";
    }

    return configured.TrimEnd('/');
}

static void RenderHeader(BackendApiService backendApiService)
{
    AnsiConsole.Clear();
    AnsiConsole.Write(new FigletText("rosslight Client").Color(Color.Green));

    var snapshot = backendApiService.HealthSnapshot;
    var statusMarkup = snapshot switch
    {
        { IsReachable: true, Status: not null }
            when string.Equals(snapshot.Status, "Healthy", StringComparison.OrdinalIgnoreCase) => "[green]Healthy[/]",
        { IsReachable: true, Status: not null } => $"[yellow]{Markup.Escape(snapshot.Status)}[/]",
        _ => "[red]Unavailable[/]",
    };

    var versionMarkup = snapshot.Version is { Length: > 0 }
        ? $"[blue]{Markup.Escape(snapshot.Version)}[/]"
        : "[grey]unknown[/]";

    var body =
        $"[grey]Backend:[/] [blue]{Markup.Escape(backendApiService.BaseAddress?.ToString() ?? "n/a")}[/]\n"
        + $"[grey]Health:[/] {statusMarkup}\n"
        + $"[grey]Version:[/] {versionMarkup}\n"
        + $"[grey]Last update:[/] [blue]{snapshot.LastUpdatedAt:HH:mm:ss}[/]";

    if (!string.IsNullOrWhiteSpace(snapshot.ErrorMessage))
    {
        body += $"\n[grey]Error:[/] [red]{Markup.Escape(snapshot.ErrorMessage)}[/]";
    }

    AnsiConsole.Write(new Panel(body).Header("Backend status").Border(BoxBorder.Rounded));
}

static async Task ListCitiesAsync(BackendApiService backendApiService)
{
    var result = await backendApiService.GetCitiesAsync();
    if (!EnsureSuccess(result))
        return;

    if (result.Value == null || result.Value.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]No cities found.[/]");
        return;
    }

    var table = new Table().Border(TableBorder.Rounded).Title("Cities");
    table.AddColumn("Id");
    table.AddColumn("Name");
    table.AddColumn("Inhabitants");

    foreach (var city in result.Value.OrderBy(city => city.Name))
    {
        table.AddRow(city.Id.ToString(), city.Name, city.InhabitantCount.ToString());
    }

    AnsiConsole.Write(table);
}

static async Task CreateCityAsync(BackendApiService backendApiService)
{
    string name = PromptRequired("City name");
    var result = await backendApiService.CreateCityAsync(name);
    if (!EnsureSuccess(result))
    {
        return;
    }

    if (result.Value == null)
    {
        AnsiConsole.MarkupLine("[yellow]City was created, but the response could not be read.[/]");
        return;
    }

    AnsiConsole.MarkupLine($"[green]Created city[/] #{result.Value.Id}: [blue]{result.Value.Name}[/]");
}

static async Task ListPersonsAsync(BackendApiService backendApiService)
{
    var result = await backendApiService.GetPersonsAsync();
    if (!EnsureSuccess(result))
    {
        return;
    }

    if (result.Value == null || result.Value.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]No persons found.[/]");
        return;
    }

    var table = new Table().Border(TableBorder.Rounded).Title("Persons");
    table.AddColumn("Id");
    table.AddColumn("First name");
    table.AddColumn("Last name");

    foreach (var person in result.Value.OrderBy(person => person.LastName).ThenBy(person => person.FirstName))
    {
        table.AddRow(person.Id.ToString(), person.FirstName, person.LastName);
    }

    AnsiConsole.Write(table);
}

static async Task CreatePersonAsync(BackendApiService backendApiService)
{
    var citiesResult = await backendApiService.GetCitiesAsync();
    if (!EnsureSuccess(citiesResult))
    {
        return;
    }

    if (citiesResult.Value == null || citiesResult.Value.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]Create a city first. Persons must belong to a city.[/]");
        return;
    }

    string firstName = PromptRequired("First name");
    string lastName = PromptRequired("Last name");
    var selectedCity = AnsiConsole.Prompt(
        new SelectionPrompt<CityMinimalResponse>()
            .Title("Select a city")
            .UseConverter(city => $"#{city.Id} {city.Name}")
            .AddChoices(citiesResult.Value.OrderBy(city => city.Name))
    );

    var result = await backendApiService.CreatePersonAsync(firstName, lastName, selectedCity.Id);
    if (result.StatusCode == 400)
    {
        AnsiConsole.MarkupLine("[red]The selected city does not exist.[/]");
        return;
    }

    if (!EnsureSuccess(result))
    {
        return;
    }

    if (result.Value == null)
    {
        AnsiConsole.MarkupLine("[yellow]Person was created, but the response could not be read.[/]");
        return;
    }

    AnsiConsole.MarkupLine(
        $"[green]Created person[/] #{result.Value.Id}: [blue]{result.Value.FirstName} {result.Value.LastName}[/] in [blue]{result.Value.City.Name}[/]"
    );
}

static bool EnsureSuccess<T>(ApiResult<T> result)
{
    if (result.IsSuccess)
    {
        return true;
    }

    AnsiConsole.MarkupLine(
        $"[red]Request failed:[/] {result.StatusCode?.ToString() ?? "Unknown"} {Markup.Escape(result.ErrorMessage ?? "Unknown error")}"
    );

    if (!string.IsNullOrWhiteSpace(result.ErrorBody))
    {
        AnsiConsole.Write(new Panel(Markup.Escape(result.ErrorBody)).Header("Response body").Border(BoxBorder.Rounded));
    }

    return false;
}

static string PromptRequired(string label)
{
    return AnsiConsole
        .Prompt(
            new TextPrompt<string>($"{label}:")
                .PromptStyle("green")
                .Validate(value =>
                    string.IsNullOrWhiteSpace(value)
                        ? ValidationResult.Error("[red]A value is required.[/]")
                        : ValidationResult.Success()
                )
        )
        .Trim();
}

static void WaitForContinue()
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
    Console.ReadKey(intercept: true);
}

file sealed record MenuAction(string Key, string Label)
{
    public static IReadOnlyList<MenuAction> All { get; } =
    [
        new("list-cities", "List cities"),
        new("create-city", "Create city"),
        new("list-persons", "List persons"),
        new("create-person", "Create person"),
        new("exit", "Exit"),
    ];
}
