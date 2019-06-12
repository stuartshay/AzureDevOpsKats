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
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        ///  Description of Cat.
        /// </summary>
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }

        /// <summary>
        /// Photo File Name.
        /// </summary>
        public string Photo { get; set; }
    }
}
