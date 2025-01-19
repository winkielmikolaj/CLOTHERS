using System.ComponentModel.DataAnnotations;

namespace Clothers.ViewModels
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Imię jest wymagane.")]
        [Display(Name = "Imię")]
        [RegularExpression(@"^[^0-9]*$", ErrorMessage = "Imię nie może zawierać cyfr.")]
        [StringLength(50, ErrorMessage = "Imię nie może przekraczać 50 znaków.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [Display(Name = "Nazwisko")]
        [RegularExpression(@"^[^0-9]*$", ErrorMessage = "Nazwisko nie może zawierać cyfr.")]
        [StringLength(50, ErrorMessage = "Nazwisko nie może przekraczać 50 znaków.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Adres dostawy jest wymagany.")]
        [Display(Name = "Adres Dostawy")]
        [StringLength(200, ErrorMessage = "Adres dostawy nie może przekraczać 200 znaków.")]
        public string DeliveryAddress { get; set; }

        [Required(ErrorMessage = "Wybierz sposób płatności.")]
        [Display(Name = "Sposób Płatności")]
        [StringLength(50, ErrorMessage = "Sposób płatności nie może przekraczać 50 znaków.")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Wybierz sposób dostawy.")]
        [Display(Name = "Sposób Dostawy")]
        [StringLength(50, ErrorMessage = "Sposób dostawy nie może przekraczać 50 znaków.")]
        public string DeliveryMethod { get; set; }
    }
}
