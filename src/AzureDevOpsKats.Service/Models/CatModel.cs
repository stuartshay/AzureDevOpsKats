using System.ComponentModel.DataAnnotations;

namespace AzureDevOpsKats.Service.Models
{
    /// <summary>
    /// Cat Model.
    /// </summary>
    public class CatModel
    {
        public long Id { get; set; }

        /// <summary>
        ///  Cat Name.
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

        /// <summary>
        /// Photo File Name.
        /// </summary>
        [Required(ErrorMessage = "The photo is required")]
        public string Photo { get; set; }
    }
}
