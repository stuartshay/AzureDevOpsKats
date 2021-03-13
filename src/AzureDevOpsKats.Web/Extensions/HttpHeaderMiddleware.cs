using AzureDevOpsKats.Web.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AzureDevOpsKats.Web.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly string _machineName;

        private readonly string _hostName;

        private readonly string _ip4AddressList;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public HttpHeaderMiddleware(RequestDelegate next)
        {
            this._next = next;
            _machineName = Environment.MachineName;
            _hostName = Dns.GetHostName();
            _ip4AddressList = ApplicationHelpers.GetIpAddressList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Response.OnStarting((Func<Task>)(() =>
            {
                httpContext.Response.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
                httpContext.Response.Headers.Add("X-MachineName", _machineName);
                httpContext.Response.Headers.Add("X-HostName", _hostName);
                httpContext.Response.Headers.Add("X-IpV4", _ip4AddressList);

                return Task.CompletedTask;
            }));
            try
            {
                await this._next(httpContext);

            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = 500;
            }

        }
    }
}
