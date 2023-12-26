using Microsoft.AspNetCore.Http.HttpResults;
using shortnr.Application.Features.UrlManagement;

namespace shortnr.Tests.Features.UrlManagement.ShortenUrl;

internal class FakeTimeService : ITimeService
{
    public long FakeValue { get; init; }

    public long Now() => FakeValue;
}

public class UnitTests
{
    private static readonly ITimeService _fakeTimeService = new FakeTimeService
    {
        FakeValue = 123
    };

    [Fact]
    public void Urls_are_shortened()
    {
        /** GIVEN
        *   A valid URL
        */
        var request = new ShortenRequest("https://foobar.com");

        /** WHEN
        *   The URL is shortened.
        */
        var results = ShortenUrlFeature.ShortenUrl(request, _fakeTimeService);

        /** THEN
        *   The URL correctly shortened
        */
        Assert.IsType<Ok<ShortenResponse>>(results.Result);

        var ok = (Ok<ShortenResponse>)results.Result;
        var response = ok.Value!;
        var uriIsValid = Uri.TryCreate(response.Url, UriKind.Absolute, out var uri);

        Assert.True(uriIsValid);
        Assert.Equal("/202-CB9", uri!.AbsolutePath);
    }

    [Theory]
    [MemberData(nameof(InvalidUrls))]
    public void Invalid_urls_are_rejected(string? invalidUrl)
    {
        /** GIVEN
        *   An invalid URL
        */
        var request = new ShortenRequest(invalidUrl);

        /** WHEN
        *   The URL is shortened.
        */
        var results = ShortenUrlFeature.ShortenUrl(request, _fakeTimeService);

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