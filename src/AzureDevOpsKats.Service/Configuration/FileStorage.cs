using System.IO;

namespace AzureDevOpsKats.Service.Configuration
{
    public class FileStorage
    {
        public string FilePath { get; set; }

        public string PhysicalFilePath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), FilePath);
            }
        }

        public string RequestPath { get; set; }
    }
}
