using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AzureDevOpsKats.Web.ViewModels
{
    /// <summary>
    /// Cat Create View Model
    /// </summary>
    public class CatCreateViewModel
    {
        /// <summary>
        /// IForm File
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// Cat Name
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Cat Description
        /// </summary>
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
    }
}
