using StackExchange.Redis;
using shortnr.WebApi;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<ConnectionMultiplexer>((_) =>
{
    var connectionString = builder.Configuration.GetValue<string>("redis:connectionString");
    ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

    return ConnectionMultiplexer.Connect(connectionString);
});

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddFeatureServices();

var app = builder.Build();

app.MapFeatureEndpoints();

app.MapHealthChecks("/health");

app.Run();

