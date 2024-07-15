using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebUI.ViewModels
{
    public class CreateViewModel
    {

        [Required]
        [DisplayName("Full Name")]
        [DataType(DataType.Text)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [DisplayName("E-Mail")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Bu mail adresi kullanılmakta.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DisplayName("Parola")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Parola Eşleşmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
