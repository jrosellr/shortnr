using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using shortnr.Application.Features.UrlManagement;

namespace shortnr.Tests.Features.UrlManagement.ShortenUrl;

internal class FakeAtomicCounter : IAtomicCounter
{
    public long FakeValue { get; set; }

    public long Next()
    {
        return FakeValue;
    }
}

public class UnitTests
{
    private static readonly FakeAtomicCounter _atomicCounter = new()
    {
        FakeValue = 123
    };

    [Fact]
    public async void Urls_are_shortened()
    {
        /** GIVEN
        *   A valid URL
        */
        var request = new ShortenRequest("https://foobar.com");

        var dynamoDb = Substitute.For<IAmazonDynamoDB>();
        dynamoDb.PutItemAsync("Url", Arg.Any<Dictionary<string, AttributeValue>>(), CancellationToken.None).Returns(Task.FromResult(new PutItemResponse()));
        var repository = new UrlRepository(dynamoDb);

        /** WHEN
        *   The URL is shortened.
        */
        var results = await ShortenUrlFeature.ShortenUrlAsync(request, _atomicCounter, repository, CancellationToken.None);

        /** THEN
        *   The URL correctly shortened
        */
        Assert.IsType<Ok<ShortenResponse>>(results.Result);

        var ok = (Ok<ShortenResponse>)results.Result;
        var response = ok.Value!;
        var uriIsValid = Uri.TryCreate(response.Url, UriKind.Absolute, out var uri);

        Assert.True(uriIsValid);
        Assert.Equal("/202-CB9", uri!.AbsolutePath);
        await dynamoDb.Received().PutItemAsync("Url", Arg.Any<Dictionary<string, AttributeValue>>(), CancellationToken.None);
    }

    [Theory]
    [MemberData(nameof(InvalidUrls))]
    public async void Invalid_urls_are_rejected(string? invalidUrl)
    {
        /** GIVEN
        *   An invalid URL
        */
        var request = new ShortenRequest(invalidUrl);

        var dynamoDb = Substitute.For<IAmazonDynamoDB>();
        var repository = new UrlRepository(dynamoDb);

        /** WHEN
        *   The URL is shortened.
        */
        var results = await ShortenUrlFeature.ShortenUrlAsync(request, _atomicCounter, repository, CancellationToken.None);

        /** THEN
        *   The URL is rejected and a BadRequest is returned.
        */
        Assert.IsType<BadRequest>(results.Result);
    }

    public static readonly IEnumerable<object?[]> InvalidUrls =
    [
        [ "foobar" ],
        [ null ],
        [ "   " ],
        [ "ftp://foobar.io" ]
    ];
}
