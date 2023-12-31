using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;


namespace shortnr.Application.Features.UrlManagement;

public record ShortenRequest(string? Url);
public record ShortenResponse(string Url);

public static class ShortenUrlFeature
{
    [UnconditionalSuppressMessage("Aot", "IL3050:RequiresDynamicCode")]
    [UnconditionalSuppressMessage("Aot", "IL2026:RequiresUnreferencedCodeAttribute")]
    public static void MapEndpoint(WebApplication app)
    {

        app.MapPost("/api/shorten", ShortenUrlAsync);
    }

    public static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IAtomicCounter, AtomicCounter>();
        services.AddScoped<IUrlRepository, UrlRepository>();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, FeatureJsonSerializerContext.Default);
        });
    }

    public static async Task<Results<Ok<ShortenResponse>, BadRequest>> ShortenUrlAsync(ShortenRequest request, IAtomicCounter atomicCounter, IUrlRepository repository, CancellationToken cancellationToken)
    {
        var isEmpty = string.IsNullOrWhiteSpace(request.Url);
        if (isEmpty)
        {
            return TypedResults.BadRequest();
        }

        var isValid = Uri.TryCreate(request.Url, UriKind.Absolute, out var uri);
        if (!isValid)
        {
            return TypedResults.BadRequest();
        }

        var hasAllowedScheme = uri is { Scheme: "http" or "https" };
        if (!hasAllowedScheme)
        {
            return TypedResults.BadRequest();
        }

        var counter = atomicCounter.Next();
        var key = UrlShortener.GenerateKey(counter);

        await repository.CreateAsync(key, counter, request.Url!, cancellationToken);

        var url = string.Format("http://localhost:5120/{0}-{1}", key[..3], key[3..6]);

        return TypedResults.Ok(new ShortenResponse(url));
    }
}

internal static class UrlShortener
{
    public static string GenerateKey(long x)
    {
        return Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(x.ToString())))
          .ToUpperInvariant()[..6];
    }
}

public interface IAtomicCounter
{
    public long Next();
}

internal class AtomicCounter : IAtomicCounter
{
    private long _counter = 0;
    private readonly object _lock = new();

    public long Next()
    {
        long current;

        lock (_lock)
        {
            current = _counter;
            _counter += 1;
        }

        return current;
    }
}

public interface IUrlRepository
{
    public Task CreateAsync(string key, long id, string url, CancellationToken cancellationToken);
}

internal class UrlRepository(IAmazonDynamoDB dynamoDb) : IUrlRepository
{
    public async Task CreateAsync(string key, long id, string url, CancellationToken cancellationToken)
    {
        var item = new Dictionary<string, AttributeValue>()
        {
            ["Key"] = new AttributeValue(key),
            ["Id"] = new AttributeValue
            {
                N = id.ToString()
            },
            ["Url"] = new AttributeValue(url)
        };

        await dynamoDb.PutItemAsync("Url", item, cancellationToken);
    }
}

[JsonSerializable(typeof(ShortenRequest))]
[JsonSerializable(typeof(ShortenResponse))]
[JsonSerializable(typeof(Task<Results<Ok<ShortenResponse>, BadRequest>>))]
internal partial class FeatureJsonSerializerContext : JsonSerializerContext { }

