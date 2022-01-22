namespace AzureDevOpsKats.Web.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class FileItemsModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int FileCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string[] Files { get; set; }
    }
}
