using Clothers.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Clothers.ViewModels
{
    public class ProductsViewModel
    {
        [Required(ErrorMessage = "Id produktu jest wymagane.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa produktu jest wymagana.")]
        [StringLength(100, ErrorMessage = "Nazwa produktu nie może przekraczać 100 znaków.")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Opis produktu jest wymagany.")]
        [StringLength(1000, ErrorMessage = "Opis produktu nie może przekraczać 1000 znaków.")]
        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena musi być większa od 0.")]
        [Precision(18, 2)]
        [Display(Name = "Cena")]
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
    }
}
