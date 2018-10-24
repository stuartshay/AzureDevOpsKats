using System.ComponentModel.DataAnnotations;

namespace AzureDevOpsKats.Service.Models
{
    public class CatModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(250)]
        public string Description { get; set; }

        public string Photo { get; set; }
    }
}
