using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AzureDevOpsKats.Common.HealthChecks
{
    /// <summary>
    /// Version Health Check
    /// </summary>
    public class VersionHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Application Version Health Check
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var applicationVersionNumber = Assembly.GetEntryAssembly()?.GetName().Version?.ToString();

            var result = Task.FromResult(string.IsNullOrEmpty(applicationVersionNumber) ? HealthCheckResult.Unhealthy("failed") : HealthCheckResult.Healthy(applicationVersionNumber));
            return result;
        }
    }
}
