namespace AzureDevOpsKats.Service.Interface
{
    public interface IFileService
    {
        void DeleteFile(string filePath);

        void SaveFile(string filePath, byte[] bytes);
    }
}
