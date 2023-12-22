using System.Security.Cryptography;
using System.Text;
using Amazon.DynamoDBv2;
using StackExchange.Redis;

namespace shortnr.WebApi.Endpoints;

public record ShortenResponse(string Url);
public record ShortenRequest(string Url);

internal static class Endpoints
{
    private static Dictionary<string, string> _shortenedUrls = new Dictionary<string, string>();

    public static async Task<IResult> Shorten(ILoggerFactory loggerFactory, AmazonDynamoDBClient dynamoClient, ConnectionMultiplexer multiplexer, ShortenRequest request)
    {
        var logger = loggerFactory.CreateLogger(nameof(Endpoints));
        using var md5 = MD5.Create();
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var hash = Convert.ToHexString(md5.ComputeHash(Encoding.UTF8.GetBytes(now.ToString())))
          .ToLower()
          .Substring(0, 5);

        if (!_shortenedUrls.ContainsKey(hash))
        {
            _shortenedUrls.Add(hash, request.Url);
            var db = multiplexer.GetDatabase();
            logger.LogWarning(multiplexer.GetStatus());
            db.StringSet(hash, request.Url);

            var tables = await dynamoClient.ListTablesAsync();
            logger.LogWarning($"{tables.TableNames.Count}");
        }

        return Results.Ok(new ShortenResponse(hash));
    }

    public static IResult Redirect(string slug)
    {
        var url = _shortenedUrls[slug];
        return Results.Redirect(url);
    }
}

