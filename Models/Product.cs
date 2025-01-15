using Clothers.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clothers.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")]
        [Display(Name = "Size")]
        public ProductSizes Sizes { get; set; }

        [Required]
        public int Quantity { get; set; }


        public byte[]? Image { get; set; }
    }
}
