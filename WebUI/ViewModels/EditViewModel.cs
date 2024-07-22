using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebUI.ViewModels
{
    public class EditViewModel
    {
        public string? Id { get; set; }
        [DisplayName("Full Name")]
        [DataType(DataType.Text)]
        public string? FullName { get; set; }

        [EmailAddress]
        [DisplayName("E-Mail")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DisplayName("Parola")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Parola Eşleşmiyor")]
        public string? ConfirmPassword { get; set; } 
    }
}
