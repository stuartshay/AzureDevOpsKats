using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureDevOpsKats.Service.Interface
{
    public interface IFileService
    {
        void ValidateDirectory(string path);

        void DeleteFile(string fileName);

        void SaveFile(string filePath, byte[] bytes);

        //async Task UploadFileAsync(string filePath, IFormFile file);
    }
}
