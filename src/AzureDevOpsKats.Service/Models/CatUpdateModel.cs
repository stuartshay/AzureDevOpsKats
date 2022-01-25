﻿using System.ComponentModel.DataAnnotations;

namespace AzureDevOpsKats.Service.Models
{
    /// <summary>
    /// Cat Update Model.
    /// </summary>
    public class CatUpdateModel
    {
        [Required(ErrorMessage = "The name is required")]
        [MaxLength(50, ErrorMessage = "The name may not exceed 50 characters")]
        [MinLength(3, ErrorMessage = "The name may be at least 3 characters")]
        public string Name { get; set; }

        /// <summary>
        ///  Description of Cat.
        /// </summary>
        [Required(ErrorMessage = "The description is required")]
        [MaxLength(65, ErrorMessage = "The description may not exceed 250 characters")]
        [MinLength(3, ErrorMessage = "The description may be at least 3 characters")]
        public string Description { get; set; }
    }
}
