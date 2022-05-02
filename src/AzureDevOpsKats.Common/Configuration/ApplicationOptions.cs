namespace AzureDevOpsKats.Common.Configuration
{
    public class ApplicationOptions
    {
        public MemoryHealthConfiguration MemoryHealthConfiguration { get; set; }

        public ApiHealthConfiguration ApiHealthConfiguration { get; set; }

        public LoggingConfiguration Logging { get; set; }

        public KeyVaultConfiguration KeyVaultConfiguration { get; set; }
    }

    public class LoggingConfiguration
    {
        public ElasticSearchConfiguration ElasticSearchConfiguration { get; set; }
    }

}
