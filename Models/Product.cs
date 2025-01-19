using Clothers.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clothers.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa jest wymagana.")]
        [StringLength(100, ErrorMessage = "Nazwa nie może przekraczać 100 znaków.")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Opis jest wymagany.")]
        [StringLength(1000, ErrorMessage = "Opis nie może przekraczać 1000 znaków.")]
        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena musi być większa od 0.")]
        [Display(Name = "Cena")]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Rozmiar jest wymagany.")]
        [Column(TypeName = "nvarchar(20)")]
        [Display(Name = "Rozmiar")]
        public ProductSizes Sizes { get; set; }

        [Required(ErrorMessage = "Ilość jest wymagana.")]
        [Range(0, int.MaxValue, ErrorMessage = "Ilość nie może być ujemna.")]
        [Display(Name = "Ilość")]
        public int Quantity { get; set; }

        [Display(Name = "Zdjęcie")]
        public byte[]? Image { get; set; }


        public bool IsApproved { get; set; }

        [Required(ErrorMessage = "Id użytkownika jest wymagane.")]
        public string UserId { get; set; }


        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
    }
}
