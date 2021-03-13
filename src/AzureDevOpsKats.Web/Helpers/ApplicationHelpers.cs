using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace AzureDevOpsKats.Web.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class ApplicationHelpers
    {
        /// <summary>
        /// Get List of IPv4 Address List
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddressList()
        {
            IPAddress[] ipv4Addresses = Array.FindAll(Dns.GetHostEntry(string.Empty).AddressList, a => a.AddressFamily == AddressFamily.InterNetwork);

            var ipList = new List<string>(ipv4Addresses.Count());
            foreach (var ipAddress in ipv4Addresses)
            {
                ipList.Add(ipAddress.ToString());
            }

            return string.Join(",", ipList);
        }
    }
}
