using System.Collections.Generic;

namespace AzureDevOpsKats.Common.Configuration
{
    public class ApiHealthConfiguration
    {
        public bool Enabled { get; set; }

        public List<Endpoint> Endpoints { get; set; }

    }

    public class Endpoint
    {
        public string Name { get; set; }

        public string Uri { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string HttpMethod { get; set; }

        public string FailureStatus { get; set; }

    }
}
