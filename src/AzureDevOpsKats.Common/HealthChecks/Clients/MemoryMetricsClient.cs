using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AzureDevOpsKats.Common.HealthChecks.Metrics;
using Microsoft.Extensions.Logging;

namespace AzureDevOpsKats.Common.HealthChecks.Clients
{

    /// <summary>
    /// 
    /// </summary>
    public class MemoryMetricsClient
    {
        public MemoryMetrics GetMetrics()
        {
            MemoryMetrics metrics;

            if (IsUnix())
            {
                metrics = GetUnixMetrics();
            }
            else
            {
                metrics = GetWindowsMetrics();
            }

            return metrics;
        }

        private bool IsUnix()
        {
            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            return isUnix;
        }

        private MemoryMetrics GetWindowsMetrics()
        {
            var output = string.Empty;
            var metrics = new MemoryMetrics(); 

            var info = new ProcessStartInfo
            {
                FileName = "wmic",
                Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value",
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(info))
            {
                if (process != null) output = process.StandardOutput.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(output))
            {
                var lines = output.Trim().Split("\n");
                var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
                var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

                metrics.Total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0);
                metrics.Free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0);
                metrics.Used = metrics.Total - metrics.Free;
            }

            return metrics;
        }

        private MemoryMetrics GetUnixMetrics()
        {
            var output = string.Empty;
            var metrics = new MemoryMetrics();

            var info = new ProcessStartInfo("free -m")
            {
                FileName = "bash", Arguments = "-c \"free -m\"", RedirectStandardOutput = true
            };

            using (var process = Process.Start(info))
            {
                if (process != null) output = process.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
            }

            if (!string.IsNullOrEmpty(output))
            {
                var lines = output.Trim().Split("\n");

                // Split Free Command Results
                var value2 = System.Text.RegularExpressions.Regex.Split(lines[1], @"\s\s+");
                
                var freeMemory = value2.Length == 7 ? value2[3] : "0";
                var totalMemory = value2.Length == 7 ? value2[1] : "0";

                metrics.Total = Math.Round(double.Parse(totalMemory) / 1024, 0);
                metrics.Free = Math.Round(double.Parse(freeMemory) / 1024, 0);
                metrics.Used = metrics.Total - metrics.Free;
            }

            Console.WriteLine($"Metrics:Total{metrics.Total}|Free:{metrics.Free}|Used:{metrics.Used}");

            return metrics;
        }
    }
}
