using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clothers.Models
{
    //class representation of user cart
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
