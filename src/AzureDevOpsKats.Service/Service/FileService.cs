using System;
using System.IO;
using AzureDevOpsKats.Service.Interface;

namespace AzureDevOpsKats.Service.Service
{
    public class FileService : IFileService, IDisposable
    {
        private readonly string _applicationPath;

        public FileService(string applicationPath)
        {
            _applicationPath = applicationPath;
        }

        public void SaveFile(string filePath, byte[] bytes)
        {
            File.WriteAllBytes(filePath, bytes);
        }

        public void DeleteFile(string fileName)
        {
            var filePath = Path.Combine($"{Path.GetFullPath(_applicationPath)}/{fileName}");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
