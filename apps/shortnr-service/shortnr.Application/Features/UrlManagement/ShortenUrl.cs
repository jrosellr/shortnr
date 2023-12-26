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
        services.AddSingleton<ITimeService, TimeService>();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, FeatureJsonSerializerContext.Default);
        });
    }

    public static Results<Ok<ShortenResponse>, BadRequest> ShortenUrl([FromBody] ShortenRequest request, ITimeService timeService)
    {
        var hasValue = string.IsNullOrWhiteSpace(request.Url);
        if (!hasValue)
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

        var now = timeService.Now();
        var shortenedUrl = UrlShortener.Shorten("http://localhost:5120/{0}", now);

        return TypedResults.Ok(new ShortenResponse(shortenedUrl));
    }
}

public interface ITimeService
{
    public long Now();
}

internal class TimeService : ITimeService
{
    public long Now() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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

[JsonSerializable(typeof(ShortenRequest))]
[JsonSerializable(typeof(ShortenResponse))]
internal partial class FeatureJsonSerializerContext : JsonSerializerContext { }
