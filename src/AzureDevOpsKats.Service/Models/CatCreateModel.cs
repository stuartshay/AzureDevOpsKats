using System.ComponentModel.DataAnnotations;

namespace AzureDevOpsKats.Service.Models
{
    /// <summary>
    /// Create Model for Cat.
    /// </summary>
    public class CatCreateModel
    {
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

        /// <summary>
        ///  Serialzied Photo
        /// </summary>
        // [Required]
        public byte[] Bytes { get; set; }

        public override string ToString()
        {
            return $"Name:{Name}|Description:{Description}";
        }
    }
}
