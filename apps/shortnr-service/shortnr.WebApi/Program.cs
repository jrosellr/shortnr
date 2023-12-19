using System.Text.Json.Serialization;
using shortnr.WebApi.Endpoints;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddHealthChecks();

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
