using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clothers.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane.")]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Adres dostawy jest wymagany.")]
        [Display(Name = "Adres Dostawy")]
        public string DeliveryAddress { get; set; }

        [Required(ErrorMessage = "Wybierz sposób płatności.")]
        [Display(Name = "Sposób Płatności")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Wybierz sposób dostawy.")]
        [Display(Name = "Sposób Dostawy")]
        public string DeliveryMethod { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public string UserId { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
