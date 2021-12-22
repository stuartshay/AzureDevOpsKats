using Elastic.Apm;
using Elastic.Apm.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AzureDevOpsKats.Web.HostedServices
{
    /// <summary>
    /// 
    /// </summary>
    public class MyCronJob2 : CronJobService
    {
        private readonly ILogger<MyCronJob2> _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public MyCronJob2(IScheduleConfig<MyCronJob2> config, ILogger<MyCronJob2> logger, IServiceProvider serviceProvider)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 2 starts.");
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task DoWork(CancellationToken cancellationToken)
        {
            //var transaction = Elastic.Apm.Agent
            //    .Tracer.StartTransaction("CronJobService", ApiConstants.TypeRequest);

            //if (Agent.Tracer.CurrentTransaction != null)
            //    Agent.Tracer.CurrentTransaction.CaptureSpan("SampleSpan", "PerfBenchmark", () => { });

            //using var scope = _serviceProvider.CreateScope();
            //var svc = scope.ServiceProvider.GetRequiredService<ICatsHostedService>();

            //await svc.DoWork(cancellationToken);
            //_logger.LogInformation($"{DateTime.Now:hh:mm:ss} CronJob 2 is working.");

            //transaction.End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CronJob 2 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
