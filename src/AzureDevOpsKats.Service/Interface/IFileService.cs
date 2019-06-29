namespace AzureDevOpsKats.Service.Interface
{
    public interface IFileService
    {
        void ValidateDirectory(string path);

        void DeleteFile(string fileName);

        void SaveFile(string filePath, byte[] bytes);
    }
}
