using shortnr.Application.Features.Navigation;
using shortnr.Application.Features.UrlManagement;

namespace shortnr.WebApi.Features;

public static class Features
{
    public static void MapFeatureEndpoints(this WebApplication app)
    {
        ShortenUrlFeature.MapEndpoint(app);
        RedirectFeature.MapEndpoint(app);
    }

    public static void AddFeatureServices(this IServiceCollection services)
    {
        ShortenUrlFeature.AddServices(services);
        RedirectFeature.AddServices(services);
    }
}