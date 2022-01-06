using System;
using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace AzureDevOpsKats.Common.HealthChecks
{
    /// <summary>
    /// Version Health Check
    /// </summary>
    public class VersionHealthCheck : IHealthCheck
    {
        private readonly string _dnsHostName;

        private readonly string _applicationVersionNumber;

        private readonly DateTime _applicationBuildDate;

        private readonly string _environment;

        public VersionHealthCheck()
        {
            _applicationVersionNumber = Assembly.GetEntryAssembly()?.GetName().Version?.ToString();
            _applicationBuildDate = GetAssemblyLastModifiedDate();
            _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _dnsHostName = Dns.GetHostName();
        }

        /// <summary>
        /// Application Version Health Check
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var data = new Dictionary<string, object>
            {
                {"BuildDate", _applicationBuildDate},
                {"BuildVersion", _applicationVersionNumber},
                {"DNS HostName", _dnsHostName},
                {"Environment", _environment},
            };

            var healthStatus = !string.IsNullOrEmpty(_applicationVersionNumber) ? HealthStatus.Healthy : HealthStatus.Degraded;
            var healthCheckResult = new HealthCheckResult(healthStatus, _applicationVersionNumber, null, data); 

            return Task.FromResult(healthCheckResult);
        }

        private DateTime GetAssemblyLastModifiedDate()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly?.Location ?? string.Empty);
            DateTime lastModified = fileInfo.LastWriteTime;

            return lastModified.ToUniversalTime();
        }
    }
}
