using System;
using System.Text;
using AzureDevOpsKats.Service.Interface;
using AzureDevOpsKats.Service.Service;
using Xunit;

namespace AzureDevOpsKats.Test.Service
{
    public class FileServiceTest
    {
        private readonly IFileService _fileService;

        private const string ApplicationPath = "../../../artifacts";

        public FileServiceTest()
        {
            _fileService = new FileService(ApplicationPath);
        }

        [Fact]
        [Trait("Category", "Intergration")]
        public void Can_Validate_Create_Path()
        {
            _fileService.ValidateDirectory(ApplicationPath);
        }


        [Fact]
        [Trait("Category", "Intergration")]
        public void Can_Save_Delete_File()
        {
            byte[] bytes = Encoding.ASCII.GetBytes("MOCK_FILE_CONTENT");

            var fileName = $"myfile_{Guid.NewGuid()}.log";
            var filePath = $"{ApplicationPath}/{fileName}";

            _fileService.ValidateDirectory(ApplicationPath);
            _fileService.SaveFile(filePath, bytes);
            _fileService.DeleteFile(fileName);

        }

    }
}
