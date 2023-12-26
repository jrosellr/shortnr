using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace shortnr.Application.Features.Navigation;

public static class RedirectFeature
{
    [UnconditionalSuppressMessage("Aot", "IL3050:RequiresDynamicCode")]
    [UnconditionalSuppressMessage("Aot", "IL2026:RequiresUnreferencedCodeAttribute")]
    public static void MapEndpoint(WebApplication app)
    {
        app.MapGet("/{*slug}", Redirect);
    }

    public static void AddServices(IServiceCollection services) { }

    public static IResult Redirect(string? slug)
    {
        return Results.NotFound();
    }
}
