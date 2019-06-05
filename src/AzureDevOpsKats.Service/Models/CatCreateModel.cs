﻿using System.ComponentModel.DataAnnotations;

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
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Cat Description
        /// </summary>
        [Required]
        [MaxLength(250)]
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