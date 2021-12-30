using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AzureDevOpsKats.Web.HostedServices
{
    /// <summary>
    /// 
    /// </summary>
    public class CatsHostedService : ICatsHostedService
    {
        private readonly ILogger<CatsHostedService> _logger;

        /// <summary>
        /// Cats Hosted Service
        /// </summary>
        /// <param name="logger"></param>
        public CatsHostedService(ILogger<CatsHostedService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CatsHostedService|Time:{time}", DateTime.Now);

            var httpClient = new HttpClient();
            await httpClient.GetAsync("https://github.com/stuartshay/AzureDevOpsKats", cancellationToken);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ICatsHostedService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DoWork(CancellationToken cancellationToken);
    }

}
