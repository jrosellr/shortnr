using System.Text.Json.Serialization;
using shortnr.WebApi.Endpoints;
using StackExchange.Redis;
using Amazon.DynamoDBv2;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddSingleton<ConnectionMultiplexer>((_) =>
{
    var connectionString = builder.Configuration.GetValue<string>("redis:connectionString");
    ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

    return ConnectionMultiplexer.Connect(connectionString);
});

builder.Services.AddSingleton<AmazonDynamoDBClient>((_) =>
{
    var config = new AmazonDynamoDBConfig();
    config.ServiceURL = "http://shortnr-database:8000";

    return new AmazonDynamoDBClient(config);
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

app.MapGroup("/api")
  .MapPost("/shorten", Endpoints.Shorten);

app.MapHealthChecks("/health");

app.MapGet("/{*slug}", Endpoints.Redirect);

app.Run();


[JsonSerializable(typeof(ShortenResponse))]
[JsonSerializable(typeof(ShortenRequest))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }
