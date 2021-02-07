using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureDevOpsKats.Common.Configuration;
using AzureDevOpsKats.Common.HealthChecks.Clients;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Common.HealthChecks
{
    /// <summary>
    /// System MemoryHealth Check
    /// </summary>
    public class SystemMemoryHealthCheck : IHealthCheck
    {
        private readonly ILogger<SystemMemoryHealthCheck> _logger;

        private readonly MemoryHealthConfiguration _memoryHealthConfiguration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        public SystemMemoryHealthCheck(ILogger<SystemMemoryHealthCheck> logger, IOptions<ApplicationOptions> settings)
        {
            _memoryHealthConfiguration = settings.Value.MemoryHealthConfiguration;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var client = new MemoryMetricsClient();
            var metrics = client.GetMetrics();
            
            var percentUsed = Math.Abs(metrics.Total) > 0 ? Math.Round(100 * metrics.Used / metrics.Total, 2) : 0;

            var status = HealthStatus.Healthy;

            var degraded = _memoryHealthConfiguration?.Degraded ?? 80;
            var unhealthy = _memoryHealthConfiguration?.Unhealthy ?? 90;
            var message = $"Healthy:{percentUsed} < {degraded}%";

            if (percentUsed > degraded || percentUsed == 0)
            {
                message = $"Degraded:{percentUsed}% Range:{unhealthy}% > {degraded}%";
                status = HealthStatus.Degraded;
            }

            if (percentUsed > unhealthy)
            {
                message = $"Unhealthy:{percentUsed}%| > {degraded} (Degraded)%";
                status = HealthStatus.Unhealthy;
            }

            var data = new Dictionary<string, object>
            {
                {"Total", metrics.Total},
                {"Used", metrics.Used},
                {"Free", metrics.Free}
            };

            _logger.LogInformation("Metrics:{@data}", data);

            var result = new HealthCheckResult(status, message, null, data);

            return await Task.FromResult(result);
        }
    }
}
