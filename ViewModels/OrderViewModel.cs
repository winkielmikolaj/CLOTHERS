using System.ComponentModel.DataAnnotations;

namespace Clothers.ViewModels
{
    public class OrderViewModel
    {
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
    }
}
