using System.Dynamic;

namespace AzureDevOpsKats.Service.Configuration
{
    public class ApplicationOptions
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public FileStorage FileStorage { get; set; }

        public ElasticConfiguration ElasticConfiguration { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Dataprotection Dataprotection { get; set; }
    }
}
