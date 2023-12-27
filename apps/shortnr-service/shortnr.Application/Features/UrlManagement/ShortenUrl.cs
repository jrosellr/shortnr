using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
        app.MapPost("/api/shorten", ShortenUrl);
    }

    public static void AddServices(IServiceCollection services)
    {
        services.AddSingleton<AtomicCounter>();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, FeatureJsonSerializerContext.Default);
        });
    }

    public static Results<Ok<ShortenResponse>, BadRequest> ShortenUrl([FromBody] ShortenRequest request, IAtomicCounter atomicCounter)
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
        var shortenedUrl = UrlShortener.Shorten("http://localhost:5120/{0}", counter);

        return TypedResults.Ok(new ShortenResponse(shortenedUrl));
    }
}

internal static class UrlShortener
{
    public static string Shorten(string format, long timestamp)
    {
        var hash = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(timestamp.ToString())))
          .ToUpperInvariant();

        return string.Format(format, $"{hash[..3]}-{hash[3..6]}");
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

[JsonSerializable(typeof(ShortenRequest))]
[JsonSerializable(typeof(ShortenResponse))]
internal partial class FeatureJsonSerializerContext : JsonSerializerContext { }
