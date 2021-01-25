namespace AzureDevOpsKats.Common.Configuration
{
    public class MemoryHealthConfiguration
    {
        public double Healthy { get; set; }

        public double Degraded { get; set; }

        public double Unhealthy { get; set; }
    }
}
