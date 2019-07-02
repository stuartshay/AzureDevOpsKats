using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AzureDevOpsKats.Service.Models.V2
{
    /// <summary>
    /// Create Model for Cat.
    /// </summary>
    public class CatCreateModelV2
    {
        /// <summary>
        ///  IFormFile Image.
        /// </summary>
        [Required(ErrorMessage = "Photo is Required")]
        public IFormFile File { get; set; }

        /// <summary>
        /// Cat Name
        /// </summary>
        [Required(ErrorMessage = "The name is required")]
        [MaxLength(50, ErrorMessage = "The name may not exceed 50 characters")]
        [MinLength(3, ErrorMessage = "The name may be at least 3 characters")]
        public string Name { get; set; }

        /// <summary>
        ///  Description of Cat.
        /// </summary>
        [Required(ErrorMessage = "The description is required")]
        [MaxLength(250, ErrorMessage = "The description may not exceed 250 characters")]
        [MinLength(3, ErrorMessage = "The description may be at least 3 characters")]
        public string Description { get; set; }

        public override string ToString()
        {
            return $"Name:{Name}|Description:{Description}";
        }
    }
}
