using System.ComponentModel.DataAnnotations;

namespace AzureDevOpsKats.Service.Models
{
    public class CatCreateModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(250)]
        public string Description { get; set; }

        // [Required]
        public byte[] Bytes { get; set; }

        public override string ToString()
        {
            return $"Name:{Name}|Description:{Description}";
        }

    }
}
