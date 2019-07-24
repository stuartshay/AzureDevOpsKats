using System.IO;
using AzureDevOpsKats.Service.Configuration;
using AzureDevOpsKats.Service.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureDevOpsKats.Service.Service
{
    public class FileService : IFileService
    {
        private readonly string _applicationPath;

        private readonly ILogger<FileService> _logger;

        public FileService(IOptions<ApplicationOptions> settings, ILogger<FileService> logger)
        {
            _applicationPath = settings.Value.FileStorage.FilePath;
            _logger = logger;
        }

        public void SaveFile(string filePath, byte[] bytes)
        {
            _logger.LogInformation("SaveFile: {FilePath}", filePath);
            File.WriteAllBytes(filePath, bytes);
        }

        public void ValidateDirectory(string path)
        {
            bool exists = Directory.Exists(path);

            if (!exists)
                Directory.CreateDirectory(path);
        }

        public void DeleteFile(string fileName)
        {
            var filePath = Path.Combine($"{Path.GetFullPath(_applicationPath)}/{fileName}");
            _logger.LogInformation("DeleteFile: {FilePath}", filePath);

            if (File.Exists(filePath))
            {
                _logger.LogInformation("FileExists: {FilePath}", filePath);
                File.Delete(filePath);
            }
            else
            {
                _logger.LogError("File Not Exists: {FilePath}", filePath);
            }
        }
    }
}
