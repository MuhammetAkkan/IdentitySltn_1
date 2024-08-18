using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebUI.ViewModels
{
    public class LoginViewModel
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir email adresi giriniz.")]

        public string Email { get; set; } = null!;
        [DataType(DataType.Password)]
        [PasswordPropertyText]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; } = true;



    }
}
