using AzureDevOpsKats.Common.Configuration;
using AzureDevOpsKats.Common.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AzureDevOpsKats.Common.HealthChecks
{
    public static class ElasticSearchHealthCheck
    {
        public static IHealthChecksBuilder AddElasticSearchHealthCheck(this IHealthChecksBuilder builder, ElasticSearchConfiguration configuration)
        {
            if (configuration.Enabled)
            {
                var elasticUrl = configuration.ElasticUrl;

                builder.AddElasticsearch(elasticUrl, name: "ElasticSearch Client", failureStatus: HealthStatus.Degraded,
                    tags: new[] { HealthCheckType.Infrastructure.ToString(), HealthCheckType.Logging.ToString(), "Port:9200" });
            }


            return builder;
        }
    }








}
