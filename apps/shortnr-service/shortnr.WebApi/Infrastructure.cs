using shortnr.Application.Library.Persistence;

namespace shortnr.WebApi;

public static class Infrastructure
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        DynamoDB.AddServices(services, configuration);
    }
}
