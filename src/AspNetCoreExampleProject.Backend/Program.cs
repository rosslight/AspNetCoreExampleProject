using AspNetCoreExampleProject.Api;
using AspNetCoreExampleProject.Backend.Database;
using AspNetCoreExampleProject.Backend.Endpoints;
using AspNetCoreExampleProject.Backend.Helpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AspNetCoreExampleProjectJsonContext.Default);
});

builder.Services.AddSingleton<IAppInformationService, AppInformationService>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<TimestampsInterceptor>();

builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(
    (serviceProvider, options) =>
    {
        options.AddInterceptors(serviceProvider.GetRequiredService<TimestampsInterceptor>());
        string connectionString =
            Environment.GetEnvironmentVariable("DB_CONNECTION")
            ?? "Host=localhost:25432;Database=postgres;Username=postgres;Password=example";

        options.UseNpgsql(connectionString);
    }
);

builder.Services.AddHealthChecks().AddCheck<DbHealthCheck>("Database");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Bil.Backend"));
}

app.MapCustomHealthCheck();

var v1Group = app.MapGroup("/v1");

v1Group.MapCityEndpoints();
v1Group.MapPersonEndpoints();

app.Run();
