using Clothers.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clothers.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        [Range(0, double.MaxValue, ErrorMessage = "Całkowita kwota musi być nieujemna.")]
        public decimal Total { get; set; }
    }

    public class CartItemViewModel
    {
        [Required(ErrorMessage = "Id produktu jest wymagane.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Nazwa produktu jest wymagana.")]
        [StringLength(100, ErrorMessage = "Nazwa produktu nie może przekraczać 100 znaków.")]
        [Display(Name = "Nazwa produktu")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena musi być większa od 0.")]
        [Display(Name = "Cena")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Ilość jest wymagana.")]
        [Range(1, int.MaxValue, ErrorMessage = "Ilość musi być przynajmniej 1.")]
        [Display(Name = "Ilość")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Całkowita cena musi być większa od 0.")]
        [Display(Name = "Razem")]
        public decimal Total { get; set; }

        public byte[]? Image { get; set; }
    }
}
