using StackExchange.Redis;
using Amazon.DynamoDBv2;
using shortnr.WebApi.Features;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<ConnectionMultiplexer>((_) =>
{
    var connectionString = builder.Configuration.GetValue<string>("redis:connectionString");
    ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

    return ConnectionMultiplexer.Connect(connectionString);
});

builder.Services.AddSingleton<AmazonDynamoDBClient>((_) =>
{
    var config = new AmazonDynamoDBConfig
    {
        ServiceURL = "http://shortnr-database:8000"
    };

    return new AmazonDynamoDBClient(config);
});

builder.Services.AddFeatureServices();

var app = builder.Build();

app.MapFeatureEndpoints();

app.MapHealthChecks("/health");

app.Run();
