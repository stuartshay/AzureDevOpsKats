using System.ComponentModel.DataAnnotations;

namespace AzureDevOpsKats.Service.Models
{
    public class CatUpdateModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
    }
}
