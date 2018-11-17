namespace AzureDevOpsKats.Service.Interface
{
    public interface IFileService
    {
        void DeleteFile(string fileName);

        void SaveFile(string filePath, byte[] bytes);
    }
}
