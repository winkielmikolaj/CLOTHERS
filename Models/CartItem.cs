using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clothers.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Ilość musi być przynajmniej 1.")]
        [Display(Name = "Ilość")]
        public int Quantity { get; set; }
    }
}
