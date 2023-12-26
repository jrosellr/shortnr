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
    public static readonly IEnumerable<object?[]> InvalidUrls = [
        [ "foobar" ],
        [ null ],
        [ "   " ],
        [ "ftp://foobar.io" ]
    ];

    [Theory]
    [MemberData(nameof(InvalidUrls))]
    public void Invalid_urls_are_rejected(string? invalidUrl)
    {
        /** GIVEN
        *   An invalid URL
        */
        var request = new ShortenRequest(invalidUrl);
        var fakeTimeService = new FakeTimeService
        {
            FakeValue = 123
        };

        /** WHEN
        *   The URL is shortened.
        */
        var results = ShortenUrlFeature.ShortenUrl(request, fakeTimeService);

        /** THEN
        *   The URL is rejected and a BadRequest is returned.
        */
        Assert.IsType<BadRequest>(results.Result);
    }
}