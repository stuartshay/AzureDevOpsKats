using System;
using System.IO;
using System.Threading.Tasks;
using AzureDevOpsKats.Service.Interface;

namespace AzureDevOpsKats.Service.Service
{
    public class FileService : IFileService
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

        public void ValidateDirectory(string path)
        {
            bool exists = Directory.Exists(path);

            if (!exists)
                Directory.CreateDirectory(path);
        }

        public void DeleteFile(string fileName)
        {
            var filePath = Path.Combine($"{Path.GetFullPath(_applicationPath)}/{fileName}");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        /*
        public async Task UploadFileAsync(string filePath, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is empty!");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
               await file.CopyToAsync(stream);
            }
        }
        */
    }
}
