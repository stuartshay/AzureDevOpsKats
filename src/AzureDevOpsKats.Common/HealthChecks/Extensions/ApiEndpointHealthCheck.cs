using System;
using System.Net.Http;
using AzureDevOpsKats.Common.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AzureDevOpsKats.Common.HealthChecks.Extensions
{
    public static class ApiEndpointHealthCheck
    {
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
    }
}
