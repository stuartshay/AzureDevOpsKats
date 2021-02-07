namespace AzureDevOpsKats.Common.Configuration
{
    public class ApplicationOptions
    {
        public MemoryHealthConfiguration MemoryHealthConfiguration { get; set; }

        public ApiHealthConfiguration ApiHealthConfiguration { get; set; }

        public ElasticSearchConfiguration ElasticSearchConfiguration { get; set; }

    }
}
