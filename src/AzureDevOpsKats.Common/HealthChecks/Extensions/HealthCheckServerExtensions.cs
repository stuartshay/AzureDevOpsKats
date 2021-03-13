using AzureDevOpsKats.Common.Configuration;
using AzureDevOpsKats.Common.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Net.Http;

namespace AzureDevOpsKats.Common.HealthChecks.Extensions
{
    public static class HealthCheckServerExtensions
    {
        public static IHealthChecksBuilder AddVersionHealthCheck(this IHealthChecksBuilder builder)
        {
            builder.AddCheck<VersionHealthCheck>("Version Health Check",
                tags: new[] { HealthCheckType.System.ToString() });

            return builder;
        }

        public static IHealthChecksBuilder AddApiEndpointHealthChecks(this IHealthChecksBuilder builder, ApiHealthConfiguration configuration)
        {
            if (configuration.Enabled)
            {
                foreach (var endpoint in configuration.Endpoints)
                {
                    var name = endpoint.Name;
                    var uri = endpoint.Uri;
                    var tags = endpoint.Tags;
                    var failureStatus = (HealthStatus)Enum.Parse(typeof(HealthStatus), endpoint.FailureStatus, true);
                    var httpMethod = new HttpMethod(endpoint.HttpMethod.ToUpper());

                    builder.AddUrlGroup(new Uri(uri), name: name, tags: tags,
                        httpMethod: httpMethod, failureStatus: failureStatus);

                }
            }

            return builder;
        }

        public static IHealthChecksBuilder AddElasticSearchHealthCheck(this IHealthChecksBuilder builder, ElasticSearchConfiguration configuration)
        {
            if (configuration.Enabled)
            {
                var elasticUrl = configuration.ElasticUrl;

                builder.AddElasticsearch(elasticUrl, name: "ElasticSearch Client", failureStatus: HealthStatus.Degraded,
                    tags: new[] { HealthCheckType.Infrastructure.ToString(), HealthCheckType.Logging.ToString(), "Port:9200" }, timeout: new TimeSpan(0,1,0));
            }


            return builder;
        }
    }
}
