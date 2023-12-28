using System.Diagnostics.CodeAnalysis;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace shortnr.Application.Library.Persistence;

public static class DynamoDB
{
    [UnconditionalSuppressMessage("Aot", "IL3050:RequiresDynamicCode")]
    [UnconditionalSuppressMessage("Aot", "IL2026:RequiresUnreferencedCodeAttribute")]
    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAmazonDynamoDB>((_) =>
        {
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = configuration.GetValue<string>("aws:dynamoDb:serviceUrl")
            };

            return new AmazonDynamoDBClient(config);
        });
    }
}
